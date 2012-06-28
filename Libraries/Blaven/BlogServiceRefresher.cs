using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;

namespace Blaven {
    public class BlogServiceRefresher {
        private BlogServiceConfig _config;

        public BlogServiceRefresher(BlogServiceConfig config) {
            _config = config;
        }

        private static ConcurrentDictionary<string, bool> _blogKeyIsUpdating = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, bool> _blogKeyHasAnyData = new ConcurrentDictionary<string, bool>();

        public bool Refresh(string blogKey, bool forceRefresh = false) {
            if(!_blogKeyIsUpdating.ContainsKey(blogKey)) {
                _blogKeyIsUpdating[blogKey] = false;
            }

            if (_blogKeyIsUpdating[blogKey]) {
                WaitForAnyData(blogKey);

                return false;
            }

            _blogKeyIsUpdating[blogKey] = true;
            
            try {
                if(_config.RefreshMode == BlogRefreshMode.Synchronously) {
                    PerformRefresh(blogKey);

                    _blogKeyHasAnyData[blogKey] = true;
                } else {
                    var task = new Task(() => {
                        PerformRefresh(blogKey);
                    });
                    task.ContinueWith(_ => {
                        _blogKeyHasAnyData[blogKey] = true;
                    });
                    task.Start();

                    WaitForAnyData(blogKey);
                }
            }
            finally {
                _blogKeyIsUpdating[blogKey] = false;
            }

            return true;
        }

        private void WaitForAnyData(string blogKey) {
            bool value = false;
            while(!_blogKeyHasAnyData.TryGetValue(blogKey, out value) || !value) {
                Thread.Sleep(250);

                using(var session = _config.DocumentStore.OpenSession()) {
                    _blogKeyHasAnyData[blogKey] = session.Query<StoreBlogRefresh>().Any(x => x.BlogKey == blogKey);
                }
            }
        }
        
        private void PerformRefresh(string blogKey) {
            try {
                // TODO: Change to commented code when Blogger API-bug is fixed:
                // http://code.google.com/p/gdata-issues/issues/detail?id=2555
                //var lastRefresh = _config.BlogStore.GetBlogLastRefresh(key);
                //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

                var bloggerSetting = _config.BloggerSettings.First(setting => setting.BlogKey == blogKey);

                var bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                bool waitForIndexes = (_config.RefreshMode == BlogRefreshMode.Synchronously);
                _config.BlogStore.Refresh(blogKey, bloggerDocument, waitForIndexes);
            }
            catch(Exception) {
                throw;
            }
            finally {

            }
        }
    }
}

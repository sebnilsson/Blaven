using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Blogger;

namespace Blaven {
    public class BlogServiceRefresher {
        private BlogServiceConfig _config;

        public BlogServiceRefresher(BlogServiceConfig config) {
            _config = config;
        }

        private static ConcurrentDictionary<string, bool> _blogKeysUpdating = new ConcurrentDictionary<string, bool>();

        public void Refresh(string blogKey, bool forceRefresh = false) {
            if(!_blogKeysUpdating.ContainsKey(blogKey)) {
                _blogKeysUpdating[blogKey] = true;
            }

            if (_blogKeysUpdating[blogKey]) {
                return;
            }

            _blogKeysUpdating[blogKey] = true;

            //var hasData = _blogKeysUpdating[blogKey];
            //if (!hasData) {
            //    using(var session = _config.DocumentStore.OpenSession()) {
            //        hasData = session.Query<StoreBlogRefresh>().Any(x => x.BlogKey == blogKey);
            //    }
            //    _blogKeysUpdating[blogKey] = hasData;
            //}

            try {
                if(_config.RefreshMode == BlogRefreshMode.Synchronously) {
                    PerformRefresh(blogKey);
                } else {
                    Task.Factory.StartNew(() => {
                        PerformRefresh(blogKey);
                    });
                }
            }
            finally {
                _blogKeysUpdating[blogKey] = false;
            }

            // Wait for indexing - if first ever refresh
            //if(!_hasDocumentStoreAnyData) {
            //    _config.BlogStore.WaitForIndexes();
            //}

            
        }

        //private void WaitForFirstData() {
        //    while(!_hasDocumentStoreAnyData) {
        //        Thread.Sleep(100);
        //    }
        //}
        
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

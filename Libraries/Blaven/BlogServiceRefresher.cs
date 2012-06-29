using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;

namespace Blaven {
    internal class BlogServiceRefresher {
        private BlogServiceConfig _config;

        public BlogServiceRefresher(BlogServiceConfig config) {
            _config = config;
        }

        private static ConcurrentDictionary<string, bool> _blogKeyHasRan = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, bool> _blogKeyIsUpdating = new ConcurrentDictionary<string, bool>();
        
        public IEnumerable<string> RefreshBlogs(IEnumerable<string> blogKeys, bool async, bool forceRefresh = false) {
            var results = new ConcurrentBag<string>();

            Parallel.ForEach(blogKeys, blogKey => {
                string updated = RefreshBlog(blogKey, async, forceRefresh);

                if(!string.IsNullOrWhiteSpace(updated)) {
                    results.Add(updated);
                }
            });

            return results.AsEnumerable();
        }

        private string RefreshBlog(string blogKey, bool async, bool forceRefresh = false) {
            if(!_blogKeyIsUpdating.ContainsKey(blogKey)) {
                _blogKeyIsUpdating[blogKey] = false;
            }

            if(!forceRefresh && _blogKeyIsUpdating[blogKey]) {
                //WaitForAnyData(blogKey);

                return null;
            }

            _blogKeyIsUpdating[blogKey] = true;

            bool isBlogRefreshed = _config.BlogStore.GetIsBlogRefreshed(blogKey, _config.CacheTime);
            if(isBlogRefreshed) {
                return null;
            }

            var updateTask = new Task(() => {
                PerformRefresh(blogKey);

                //_blogKeyHasAnyData[blogKey] = true;
            });
            
            try {
                updateTask.Start();
                if(!async) {
                    updateTask.Wait();
                }

                //WaitForAnyData(blogKey);
            }
            finally {
                _blogKeyIsUpdating[blogKey] = false;
            }

            return blogKey;
        }

        //private void WaitForAnyData(string blogKey) {
        //    if(_blogKeyHasRan.ContainsKey(blogKey) && _blogKeyHasRan[blogKey]) {
        //        return;
        //    }

        //    var task = new Task(() => {
        //        bool value = false;
        //        while(!_blogKeyHasRan.TryGetValue(blogKey, out value) || !value) {
        //            Thread.Sleep(200);

        //            using(var session = _config.DocumentStore.OpenSession()) {
        //                _blogKeyHasRan[blogKey] = session.Query<StoreBlogRefresh>().Any(x => x.BlogKey == blogKey);
        //            }
        //        }
        //    });

        //    task.Start();
        //    bool success = task.Wait(30 * 1000);

        //    if(!success) {
        //        throw new TimeoutException();
        //    }
        //}
        
        private void PerformRefresh(string blogKey) {
            try {
                // TODO: Change to commented code when Blogger API-bug is fixed:
                // http://code.google.com/p/gdata-issues/issues/detail?id=2555
                //var lastRefresh = _config.BlogStore.GetBlogLastRefresh(key);
                //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

                var bloggerSetting = _config.BloggerSettings.First(x => x.BlogKey == blogKey);

                var bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                _config.BlogStore.Refresh(blogKey, bloggerDocument);
            }
            catch(Exception) {
                throw;
            }
            finally {

            }
        }
    }
}

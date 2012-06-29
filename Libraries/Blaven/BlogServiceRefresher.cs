using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Blogger;
using System.Threading;

namespace Blaven {
    internal class BlogServiceRefresher {
        private BlogServiceConfig _config;

        public BlogServiceRefresher(BlogServiceConfig config) {
            _config = config;
        }

        private static ConcurrentDictionary<string, bool> _blogKeyHasRan = new ConcurrentDictionary<string, bool>();
        private static ConcurrentDictionary<string, bool> _blogKeyIsUpdating = new ConcurrentDictionary<string, bool>();

        public IEnumerable<Tuple<string, bool>> RefreshBlogs(IEnumerable<string> blogKeys, bool forceRefresh = false) {
            var results = new ConcurrentBag<Tuple<string, bool>>();

            Parallel.ForEach(blogKeys, blogKey => {
                var updated = RefreshBlog(blogKey, forceRefresh);

                if(updated != null) {
                    results.Add(updated);
                }
            });

            var waitTasks = from updatedBlog in results.Where(x => x.Item2)
                            select Task.Factory.StartNew(() => {
                                while(!_config.BlogStore.GetHasBlogAnyData(updatedBlog.Item1)) {
                                    Thread.Sleep(100);
                                }
                            });

            Task.WaitAll(waitTasks.ToArray());

            return results.AsEnumerable();
        }

        private Tuple<string, bool> RefreshBlog(string blogKey, bool async, bool forceRefresh = false) {
            if(!_blogKeyIsUpdating.ContainsKey(blogKey)) {
                _blogKeyIsUpdating[blogKey] = false;
            }

            if(!forceRefresh && _blogKeyIsUpdating[blogKey]) {
                return new Tuple<string,bool>(blogKey, false);
            }

            _blogKeyIsUpdating[blogKey] = true;

            bool isBlogRefreshed = _config.BlogStore.GetIsBlogRefreshed(blogKey, _config.CacheTime);
            if(isBlogRefreshed) {
                return new Tuple<string, bool>(blogKey, false);
            }

            var updateTask = new Task(() => {
                PerformRefresh(blogKey);
            });
            
            try {
                updateTask.Start();
            }
            finally {
                _blogKeyIsUpdating[blogKey] = false;
            }

            return new Tuple<string, bool>(blogKey, true);
        }
        
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

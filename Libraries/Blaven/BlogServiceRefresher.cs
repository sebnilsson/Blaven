using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Blogger;

namespace Blaven {
    internal class BlogServiceRefresher {
        private readonly int _timeOutSeconds = 30;
        private BlogServiceConfig _config;

        public BlogServiceRefresher(BlogServiceConfig config) {
            _config = config;
        }

        private static ConcurrentDictionary<string, bool> _blogKeyIsUpdating = new ConcurrentDictionary<string, bool>();

        public IEnumerable<Tuple<string, BlogServiceRefresherResult>> RefreshBlogs(IEnumerable<string> blogKeys, bool forceRefresh = false) {
            var results = new ConcurrentBag<Tuple<string, BlogServiceRefresherResult>>();

            Parallel.ForEach(blogKeys, blogKey => {
                var updated = RefreshBlog(blogKey, forceRefresh);

                if(updated != null) {
                    results.Add(updated);
                }
            });

            var waitTasks = from updatedBlog in results.Where(x => x.Item2 != BlogServiceRefresherResult.IsRefreshed)
                            let blogKey = updatedBlog.Item1
                            select Task.Factory.StartNew(() => {
                                while(!_config.BlogStore.GetHasBlogAnyData(blogKey)) {
                                    Thread.Sleep(100);
                                }
                            });

            Task.WaitAll(waitTasks.ToArray(), TimeSpan.FromSeconds(_timeOutSeconds));

            return results.AsEnumerable();
        }
        
        private static object _refreshLock = new object();
        private Tuple<string, BlogServiceRefresherResult> RefreshBlog(string blogKey, bool async, bool forceRefresh = false) {
            lock(_refreshLock) {
                if(!_blogKeyIsUpdating.ContainsKey(blogKey)) {
                    _blogKeyIsUpdating[blogKey] = false;
                }

                if(!forceRefresh && _blogKeyIsUpdating[blogKey]) {
                    return new Tuple<string, BlogServiceRefresherResult>(blogKey, BlogServiceRefresherResult.IsUpdating);
                }

                _blogKeyIsUpdating[blogKey] = true;

                bool isBlogRefreshed = _config.BlogStore.GetIsBlogRefreshed(blogKey, _config.CacheTime);
                if(isBlogRefreshed) {
                    return new Tuple<string, BlogServiceRefresherResult>(blogKey, BlogServiceRefresherResult.IsRefreshed);
                }
            }
            
            try {
                var updateTask = Task.Factory.StartNew(() => {
                    PerformRefresh(blogKey);
                });

                if(!_config.BlogStore.GetHasBlogAnyData(blogKey)) {
                    updateTask.Wait(TimeSpan.FromSeconds(_timeOutSeconds));
                }
            }
            finally {
                _blogKeyIsUpdating[blogKey] = false;
            }

            return new Tuple<string, BlogServiceRefresherResult>(blogKey, BlogServiceRefresherResult.WasUpdated);
        }

        private void PerformRefresh(string blogKey) {
            // TODO: Change to commented code when Blogger API-bug is fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555
            //var lastRefresh = _config.BlogStore.GetBlogLastRefresh(key);
            //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

            var bloggerSetting = _config.BloggerSettings.First(x => x.BlogKey == blogKey);

            try {
                var bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                _config.BlogStore.Refresh(blogKey, bloggerDocument);
            }
            catch(BloggerServiceException) {
                if(!_config.IgnoreBloggerServiceFailure) {
                    throw;
                }
            }
        }
    }
}

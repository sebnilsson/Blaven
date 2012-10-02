using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;

namespace Blaven {
    internal static class BlogServiceRefresher {
        private static readonly int _ravenTimeoutSeconds = 10;
        private static readonly int _refreshTimeoutSeconds = 30;

        private static Dictionary<string, bool> _blogKeyIsRefreshing = new Dictionary<string, bool>();

        public static IEnumerable<RefreshResult> RefreshBlogs(RavenDbBlogStore blogStore, IEnumerable<BloggerSetting> bloggerSettings, int cacheTime, bool forceRefresh = false) {
            var results = new List<RefreshResult>();

            Parallel.ForEach(bloggerSettings, bloggerSetting => {
                var updateResult = RefreshBlog(blogStore, bloggerSetting, cacheTime, forceRefresh: forceRefresh);
                results.Add(updateResult);
            });

            return results.AsEnumerable();
        }

        private static object _refreshLock = new object();
        private static RefreshResult RefreshBlog(RavenDbBlogStore blogStore, BloggerSetting bloggerSetting, int cacheTime, bool forceRefresh = false) {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            bool isBlogRefreshed = false;
            bool isBlogRefreshing = false;

            string blogKey = bloggerSetting.BlogKey;

            lock(_refreshLock) {
                if(!_blogKeyIsRefreshing.ContainsKey(blogKey)) {
                    _blogKeyIsRefreshing[blogKey] = false;
                }

                isBlogRefreshing = _blogKeyIsRefreshing[blogKey];
                isBlogRefreshed = blogStore.GetIsBlogRefreshed(blogKey, cacheTime);
            }

            if(!forceRefresh && isBlogRefreshed) {
                return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.CancelledIsRefreshed);
            }

            if(isBlogRefreshing) {
                if(!blogStore.GetHasBlogAnyData(blogKey)) {
                    WaitForData(blogStore, blogKey);
                }

                return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.CancelledIsRefreshing);
            }

            try {
                _blogKeyIsRefreshing[blogKey] = true;

                var updateTask = new Task(() => {
                    PerformRefresh(blogStore, bloggerSetting);
                });
                updateTask.Start();

                if(forceRefresh || !blogStore.GetHasBlogAnyData(blogKey)) {
                    var isUpdatedSuccess = updateTask.Wait(TimeSpan.FromSeconds(_refreshTimeoutSeconds));
                    
                    if(isUpdatedSuccess) {
                        WaitForData(blogStore, blogKey);

                        return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.UpdateSync);
                    } else {
                        return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.UpdateFailed);
                    }
                }
            }
            finally {
                _blogKeyIsRefreshing[blogKey] = false;
            }

            return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.UpdateAsync);
        }

        private static TimeSpan StopAndGetTime(System.Diagnostics.Stopwatch stopwatch) {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static void WaitForData(RavenDbBlogStore blogStore, string blogKey) {
            var waitTask = new Task(() => {
                while(!blogStore.GetHasBlogAnyData(blogKey)) {
                    Thread.Sleep(200);
                }
            });

            waitTask.Start();
            waitTask.Wait(TimeSpan.FromSeconds(_ravenTimeoutSeconds));
        }
        
        private static void PerformRefresh(RavenDbBlogStore blogStore, BloggerSetting bloggerSetting) {
            // TODO: Change to commented code when Blogger API-bug is fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555
            //var lastRefresh = _blogStore.GetBlogLastRefresh(key);
            //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

            string blogKey = bloggerSetting.BlogKey;

            try {
                string bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                var parsedData = BloggerParser.ParseBlogData(bloggerSetting, bloggerDocument);

                var blogStoreRefreshLock = GetBlogStoreRefreshLock(blogKey);
                lock(_blogStoreRefreshLocksLock) {
                    blogStore.Refresh(blogKey, parsedData);
                }
            }
            catch(BloggerServiceException) {
                if(blogStore.GetHasBlogAnyData(blogKey)) {
                    blogStore.UpdateStoreRefresh(blogKey);
                }
            }
        }

        private static object _blogStoreRefreshLocksLock = new object();
        private static Dictionary<string, object> _blogStoreRefreshLocks = new Dictionary<string, object>();
        private static object GetBlogStoreRefreshLock(string blogKey) {
            lock(_blogStoreRefreshLocksLock) {
                object keySyncRoot;

                if(!_blogStoreRefreshLocks.TryGetValue(blogKey, out keySyncRoot)) {
                    keySyncRoot = new object();
                    _blogStoreRefreshLocks[blogKey] = keySyncRoot;
                }

                return keySyncRoot;
            }
        }
    }
}

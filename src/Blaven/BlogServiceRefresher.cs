using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;

namespace Blaven
{
    internal static class BlogServiceRefresher
    {
        private const int RavenTimeoutSeconds = 10;

        private const int RefreshTimeoutSeconds = 30;

        private static readonly object RefreshLock = new object();

        private static readonly object BlogStoreRefreshLocksLock = new object();

        ////private static Dictionary<string, object> blogStoreRefreshLocks = new Dictionary<string, object>();

        private static Dictionary<string, bool> blogKeyIsRefreshing = new Dictionary<string, bool>();

        public static IEnumerable<RefreshResult> RefreshBlogs(
            RavenDbBlogStore blogStore,
            IEnumerable<BloggerSetting> bloggerSettings,
            int cacheTime,
            bool forceRefresh = false)
        {
            var results = new List<RefreshResult>();

            Parallel.ForEach(
                bloggerSettings,
                bloggerSetting =>
                    {
                        var updateResult = RefreshBlog(blogStore, bloggerSetting, cacheTime, forceRefresh);
                        results.Add(updateResult);
                    });

            return results.AsEnumerable();
        }

        private static RefreshResult RefreshBlog(
            RavenDbBlogStore blogStore, BloggerSetting bloggerSetting, int cacheTime, bool forceRefresh = false)
        {
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            bool isBlogRefreshed;
            bool isBlogRefreshing;

            string blogKey = bloggerSetting.BlogKey;

            lock (RefreshLock)
            {
                if (!blogKeyIsRefreshing.ContainsKey(blogKey))
                {
                    blogKeyIsRefreshing[blogKey] = false;
                }

                isBlogRefreshing = blogKeyIsRefreshing[blogKey];
                isBlogRefreshed = blogStore.GetIsBlogRefreshed(blogKey, cacheTime);
            }

            if (!forceRefresh && isBlogRefreshed)
            {
                return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.CancelledIsRefreshed);
            }

            if (isBlogRefreshing)
            {
                if (!blogStore.GetHasBlogAnyData(blogKey))
                {
                    WaitForData(blogStore, blogKey);
                }

                return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.CancelledIsRefreshing);
            }

            try
            {
                blogKeyIsRefreshing[blogKey] = true;

                var updateTask = new Task(() => PerformRefresh(blogStore, bloggerSetting));
                updateTask.Start();

                if (forceRefresh || !blogStore.GetHasBlogAnyData(blogKey))
                {
                    var isUpdatedSuccess = updateTask.Wait(TimeSpan.FromSeconds(RefreshTimeoutSeconds));
                    var refreshType = RefreshType.UpdateFailed;

                    if (isUpdatedSuccess)
                    {
                        WaitForData(blogStore, blogKey);

                        refreshType = RefreshType.UpdateSync;
                    }
                    return new RefreshResult(blogKey, StopAndGetTime(stopwatch), refreshType);
                }
            }
            finally
            {
                blogKeyIsRefreshing[blogKey] = false;
            }

            return new RefreshResult(blogKey, StopAndGetTime(stopwatch), RefreshType.UpdateAsync);
        }

        private static TimeSpan StopAndGetTime(System.Diagnostics.Stopwatch stopwatch)
        {
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        private static void WaitForData(RavenDbBlogStore blogStore, string blogKey)
        {
            var waitTask = new Task(
                () =>
                    {
                        while (!blogStore.GetHasBlogAnyData(blogKey))
                        {
                            Thread.Sleep(200);
                        }
                    });

            waitTask.Start();
            waitTask.Wait(TimeSpan.FromSeconds(RavenTimeoutSeconds));
        }

        private static void PerformRefresh(RavenDbBlogStore blogStore, BloggerSetting bloggerSetting)
        {
            // TODO: Change to commented code when Blogger API-bug is fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555
            //var lastRefresh = _blogStore.GetBlogLastRefresh(key);
            //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

            string blogKey = bloggerSetting.BlogKey;

            try
            {
                string bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                var parsedData = BloggerParser.ParseBlogData(bloggerSetting, bloggerDocument);

                ///// var blogStoreRefreshLock = GetBlogStoreRefreshLock(blogKey);
                lock (BlogStoreRefreshLocksLock)
                {
                    blogStore.Refresh(blogKey, parsedData);
                }
            }
            catch (BloggerServiceException)
            {
                if (blogStore.GetHasBlogAnyData(blogKey))
                {
                    blogStore.UpdateStoreRefresh(blogKey);
                }
            }
        }

        ////private static object GetBlogStoreRefreshLock(string blogKey)
        ////{
        ////    lock (BlogStoreRefreshLocksLock)
        ////    {
        ////        object keySyncRoot;

        ////        if (!blogStoreRefreshLocks.TryGetValue(blogKey, out keySyncRoot))
        ////        {
        ////            keySyncRoot = new object();
        ////            blogStoreRefreshLocks[blogKey] = keySyncRoot;
        ////        }

        ////        return keySyncRoot;
        ////    }
        ////}
    }
}
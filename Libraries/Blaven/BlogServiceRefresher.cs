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

        private static ConcurrentDictionary<string, bool> _blogKeyIsRefreshing = new ConcurrentDictionary<string, bool>();

        public IEnumerable<string> RefreshBlogs(IEnumerable<string> blogKeys, bool forceRefresh = false) {
            var results = new ConcurrentBag<string>();

            Parallel.ForEach(blogKeys, blogKey => {
                var updated = RefreshBlog(blogKey, forceRefresh: forceRefresh);

                if(!string.IsNullOrWhiteSpace(updated)) {
                    results.Add(updated);
                }
            });

            return results.AsEnumerable();
        }

        private static object _refreshLock = new object();
        private string RefreshBlog(string blogKey, bool forceRefresh = false) {
            bool isBlogRefreshed = false;
            bool isBlogRefreshing = false;

            lock(_refreshLock) {
                if(!_blogKeyIsRefreshing.ContainsKey(blogKey)) {
                    _blogKeyIsRefreshing[blogKey] = false;
                }

                isBlogRefreshing = _blogKeyIsRefreshing[blogKey];
                isBlogRefreshed = _config.BlogStore.GetIsBlogRefreshed(blogKey, _config.CacheTime);
            }

            if(!forceRefresh && isBlogRefreshed) {
                return null;
            }

            if(isBlogRefreshing) {
                if(!_config.BlogStore.GetHasBlogAnyData(blogKey)) {
                    WaitForData(blogKey);
                }

                return null;
            }

            try {
                _blogKeyIsRefreshing[blogKey] = true;

                var updateTask = Task.Factory.StartNew(() => {
                    PerformRefresh(blogKey);
                });

                if(forceRefresh || !_config.BlogStore.GetHasBlogAnyData(blogKey)) {
                    updateTask.Wait();

                    WaitForData(blogKey);
                }
            }
            finally {
                _blogKeyIsRefreshing[blogKey] = false;
            }

            return blogKey;
        }

        private void WaitForData(string blogKey) {
            var waitTask = new Task(() => {
                while(!_config.BlogStore.GetHasBlogAnyData(blogKey)) {
                    Thread.Sleep(200);
                }
            });

            waitTask.Start();
            waitTask.Wait(TimeSpan.FromSeconds(_timeOutSeconds));
        }

        private void PerformRefresh(string blogKey) {
            // TODO: Change to commented code when Blogger API-bug is fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555
            //var lastRefresh = _config.BlogStore.GetBlogLastRefresh(key);
            //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

            var bloggerSetting = _config.BloggerSettings.First(x => x.BlogKey == blogKey);

            try {
                var bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                var parsedData = BloggerParser.ParseBlogData(blogKey, bloggerDocument, _config.ReformatBloggerParagraphs);

                _config.BlogStore.Refresh(blogKey, parsedData);
            }
            catch(BloggerServiceException) {
                if(!_config.IgnoreBloggerServiceFailure) {
                    throw;
                }

                _config.BlogStore.UpdateStoreRefresh(blogKey);
            }
        }
    }
}

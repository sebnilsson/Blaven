using System;
using System.Collections.Generic;

using BloggerViewController.Configuration;

namespace BloggerViewController {
    public class BlogServiceBlog {
        private BlogServiceConfig _config;
        private BloggerSetting _setting;

        internal BlogServiceBlog(BloggerSetting setting, BlogServiceConfig config) {
            _setting = setting;
            _config = config;
        }
        
        /// <summary>
        /// Gets the BlogInfo for the blog.
        /// </summary>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo() {
            EnsureBlogIsUpdated();

            var info = _config.BlogStore.GetBlogInfo(_setting.BlogKey);
            return info;
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination. Must have a value of 0 or higher.</param>
        /// <param name="pageSize">Optional page-size of the pagination. Defaults to value in configuration.</param>
        /// <param name="blogKey">Optional key of the blog to get the selection from. Defaults to use all blogs' blog-posts.</param>
        /// <param name="predicate">Optional predicate for filtering the blog-posts in selection.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetSelection(int pageIndex, string labelFilter = null, DateTime? dateTimeFilter = null) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            EnsureBlogIsUpdated();
            
            return _config.BlogStore.GetBlogSelection(_setting.BlogKey, pageIndex, _config.PageSize, labelFilter, dateTimeFilter);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="permaLink">The perma-link of the blog-post to get.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string permaLink) {
            EnsureBlogIsUpdated();

            return _config.BlogStore.GetBlogPost(permaLink, _setting.BlogKey);
        }

        public Dictionary<string, int> GetLabels() {
            EnsureBlogIsUpdated();

            return _config.BlogStore.GetBlogLabels(_setting.BlogKey);
        }

        public Dictionary<DateTime, int> GetPostDates() {
            EnsureBlogIsUpdated();
            
            return _config.BlogStore.GetBlogPostDates(_setting.BlogKey);
        }

        /// <summary>
        /// Updates the blog.
        /// </summary>
        public void Update() {
            // TODO: Change to commented code when Blogger API-bug is fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555
            //var lastUpdate = _config.BlogStore.GetBlogLastUpdate(_setting.BlogKey);
            //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastUpdate);
            
            var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting);

            _config.BlogStore.Update(_setting.BlogKey, bloggerDocument);
        }

        private static readonly object _lockStoreLock = new  object();
        private static Dictionary<string, object> _lockStore = new Dictionary<string, object>();
        private static object GetLock(string key) {
            if(!_lockStore.ContainsKey(key)) {
                lock(_lockStoreLock) {
                    if(!_lockStore.ContainsKey(key)) {
                        _lockStore[key] = new object();
                    }
                }
            }
            return _lockStore[key];
        }
        private void EnsureBlogIsUpdated() {
            // If the app uses background-service then don't handle update
            if(AppSettingsService.UseBackgroundService) {
                return;
            }

            if(_config.BlogStore.GetIsBlogUpdated(_setting.BlogKey)) {
                return;
            }

            var lockObject = GetLock(_setting.BlogKey);
            lock(lockObject) {
                if(_config.BlogStore.GetIsBlogUpdated(_setting.BlogKey)) {
                    return;
                }

                Update();
            }
        }
    }
}

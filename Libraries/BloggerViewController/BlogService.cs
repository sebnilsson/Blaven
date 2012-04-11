using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Configuration;
using BloggerViewController.Data;

namespace BloggerViewController {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService {
        private IEnumerable<BloggerSetting> _settings;
        private IBlogStore _store;
        private BloggerHelper _bloggerHelper;

        public const string DefaultBlogKey = "_";

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="store">The store to use for storing blog-data.</param>
        /// <param name="settings">The Blogger-settings to use in the service.</param>
        public BlogService(IBlogStore store, params BloggerSetting[] settings) {
            if(store == null) {
                throw new ArgumentNullException("store");
            }

            if(settings == null) {
                throw new ArgumentNullException("settings");
            } else if(!settings.Any()) {
                throw new ArgumentOutOfRangeException("settings", "The provided array of settings cannot be empty.");
            }

            _settings = settings.AsEnumerable();
            _bloggerHelper = new BloggerHelper();
            _store = store;
        }

        /// <summary>
        /// Gets the BlogInfo for the specified blogKey. If no parameters defaults to first blog in settings.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info for. Defaults to first blog in settings.</param>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo(string blogKey = null) {
            blogKey = blogKey ?? GetFirstBlogKey();
            EnsureBlogIsUpdated(blogKey);

            var info = _store.GetBlogInfo(blogKey);
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
        public BlogSelection GetSelection(int pageIndex, int? pageSize = null, string blogKey = null, Func<BlogPost, bool> predicate = null) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            var keys = GetBlogKeys(blogKey);

            foreach(var key in keys) {
                EnsureBlogIsUpdated(key);
            }

            var actualPageSize = pageSize.GetValueOrDefault(AppSettingsService.PageSize);

            var selection = _store.GetBlogSelection(pageIndex, actualPageSize, keys, predicate);
            return selection;
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="permaLink">The perma-link of the blog-post to get.</param>
        /// <param name="blogKey">Optional key for the blog to get the blog-post from.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string permaLink, string blogKey = null) {
            blogKey = blogKey ?? GetFirstBlogKey();
            EnsureBlogIsUpdated(blogKey);

            var post = _store.GetBlogPost(permaLink, blogKey);
            return post;
        }

        /// <summary>
        /// Updates the given blog.
        /// </summary>
        /// <param name="blogKey">Optional key for the blog to update. Defaults to update all blogs, if no value given.</param>
        public void UpdateBlogs(string blogKey = null) {
            var keys = GetBlogKeys(blogKey);

            foreach(var key in keys) {
                var setting = _settings.First(s => s.BlogKey == key);
                var bloggerDocument = _bloggerHelper.GetBloggerDocument(setting);
                _store.Update(bloggerDocument, key);
            }
        }

        private string GetFirstBlogKey() {
            return BloggerSettingsService.Settings.FirstOrDefault().BlogKey;
        }

        private IEnumerable<string> GetBlogKeys(string blogKey) {
            if(string.IsNullOrWhiteSpace(blogKey)) {
                return BloggerSettingsService.Settings.Select(setting => setting.BlogKey);
            }

            return new[] { (BloggerSettingsService.Settings.FirstOrDefault(setting => setting.BlogKey == blogKey)
                ?? BloggerSettingsService.Settings.FirstOrDefault()).BlogKey };
        }

        private static Dictionary<string, object> _locks = new Dictionary<string, object>();
        private static object GetLock(string blogKey) {
            object objLock;
            if(!_locks.TryGetValue(blogKey, out objLock)) {
                objLock = _locks[blogKey] = new object();
            }
            return objLock;
        }

        private void EnsureBlogIsUpdated(string blogKey) {
            if(AppSettingsService.UseBackgroundService) {
                return;                
            }

            if(_store.GetIsBlogUpdated(blogKey)) {
                return;
            }

            var objLock = GetLock(blogKey);
            lock(objLock) {
                if(_store.GetIsBlogUpdated(blogKey)) {
                    return;
                }
                
                UpdateBlogs(blogKey);
            }
        }
    }
}

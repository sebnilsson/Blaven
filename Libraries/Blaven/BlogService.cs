using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Blogger;

namespace Blaven {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService : IDisposable {
        private BlogServiceRefresher _refresher;

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features. Uses the default values in AppConfig.
        /// </summary>
        /// <param name="settings">The Blogger-settings to use.</param>
        public BlogService()
            : this(new BlogServiceConfig()) {

        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="settings">The Blogger-settings to use.</param>
        public BlogService(IEnumerable<BloggerSetting> settings)
            : this(new BlogServiceConfig(settings)) {

        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="config">The Blogger-settings to use in the service.</param>
        public BlogService(BlogServiceConfig config) {
            if(config == null) {
                throw new ArgumentNullException("config");
            }

            _refresher = new BlogServiceRefresher(config);

            this.Config = config;

            EnsureBlogsRefreshed(GetKeysOrAll());
        }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        //public void Init() {
        //    var blogKeys = GetKeysOrAll();

        //    Refresh( blogKeys);
        //}

        public Dictionary<DateTime, int> GetArchiveCount(params string[] blogKeys) {
            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.GetBlogArchiveCount(blogKeys);
        }

        public BlogSelection GetArchiveSelection(DateTime date, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.GetBlogArchiveSelection(date, pageIndex, this.Config.PageSize, blogKeys);
        }
        
        /// <summary>
        /// Gets the BlogInfo for a blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo(string blogKey = null) {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.Config.BlogStore.GetBlogInfo(blogKey);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="permaLink">The perma-link of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string permaLink, string blogKey = null) {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.Config.BlogStore.GetBlogPost(permaLink, blogKey);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination. Must have a value of 0 or higher.</param>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetSelection(int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.GetBlogSelection(pageIndex, this.Config.PageSize, blogKeys);
        }

        /// <summary>
        /// Gets the blog-tags count.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        /// <returns>Returns a dictionary of tags and their count.</returns>
        public Dictionary<string, int> GetTagsCount(params string[] blogKeys) {
            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.GetBlogTagsCount(blogKeys);
        }

        public BlogSelection GetTagsSelection(string tagName, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.GetBlogTagsSelection(tagName, pageIndex, this.Config.PageSize, blogKeys);
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }
            
            blogKeys = GetKeysOrAll(blogKeys);

            return this.Config.BlogStore.SearchPosts(searchTerms ?? string.Empty, pageIndex, this.Config.PageSize, blogKeys);
        }
        
        /// <summary>
        /// Refreshes blogs.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        public IEnumerable<string> Refresh(params string[] blogKeys) {
            return Refresh(forceRefresh: false, blogKeys: blogKeys);
        }

        /// <summary>
        /// Refreshes blogs.
        /// </summary>
        /// <param name="forceRefresh">Sets if the blog should be forced to refresh, ignoring if Blog is up to date.</param>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs.</param>
        public IEnumerable<string> Refresh(bool forceRefresh = false, params string[] blogKeys) {
            blogKeys = GetKeysOrAll(blogKeys);

            var updatedBlogs = _refresher.RefreshBlogs(blogKeys, forceRefresh: forceRefresh);

            if(forceRefresh) {
                this.Config.BlogStore.WaitForIndexes();
            }

            return updatedBlogs;
        }
        
        private void EnsureBlogsRefreshed(params string[] blogKeys) {
            // If the app uses background-service then don't handle refresh
            if(!this.Config.EnsureBlogsRefreshed) {
                return;
            }

            Refresh(forceRefresh: false, blogKeys: blogKeys);
        }

        private string GetBlogKeyOrDefault(string blogKey) {
            if(!string.IsNullOrWhiteSpace(blogKey)) {
                return blogKey;
            }

            return this.Config.BloggerSettings.First().BlogKey;
        }

        internal string[] GetKeysOrAll(string[] blogKeys = null) {
            blogKeys = blogKeys ?? Enumerable.Empty<string>().ToArray();

            if(blogKeys.Any()) {
                return blogKeys;
            }

            return this.Config.BloggerSettings.Select(setting => setting.BlogKey).ToArray();
        }

        #region IDisposable Members

        public void Dispose() {
            if(this.Config.DocumentStore != null && !this.Config.DocumentStore.WasDisposed) {
                this.Config.DocumentStore.Dispose();
            }
        }

        #endregion
    }
}

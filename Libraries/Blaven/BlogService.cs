using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;

namespace Blaven {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService : IDisposable {
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

            this.Config = config;

            EnsureBlogsRefreshed(this.Config.BloggerSettings.Select(setting => setting.BlogKey).ToArray());
        }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        public Dictionary<DateTime, int> GetArchiveCount(params string[] blogKeys) {
            blogKeys = GetAllKeys(blogKeys);

            return this.Config.BlogStore.GetBlogArchiveCount(blogKeys);
        }

        public BlogSelection GetArchiveSelection(DateTime date, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetAllKeys(blogKeys);

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

            blogKeys = GetAllKeys(blogKeys);

            return this.Config.BlogStore.GetBlogSelection(pageIndex, this.Config.PageSize, blogKeys);
        }

        /// <summary>
        /// Gets the blog-tags count.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        /// <returns>Returns a dictionary of tags and their count.</returns>
        public Dictionary<string, int> GetTagsCount(params string[] blogKeys) {
            blogKeys = GetAllKeys(blogKeys);

            return this.Config.BlogStore.GetBlogTagsCount(blogKeys);
        }

        public BlogSelection GetTagsSelection(string tagName, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetAllKeys(blogKeys);

            return this.Config.BlogStore.GetBlogTagsSelection(tagName, pageIndex, this.Config.PageSize, blogKeys);
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }
            
            blogKeys = GetAllKeys(blogKeys);

            return this.Config.BlogStore.SearchPosts(searchTerms ?? string.Empty, pageIndex, this.Config.PageSize, blogKeys);
        }
        
        /// <summary>
        /// Refreshes blogs.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        public void Refresh(params string[] blogKeys) {
            Refresh(forceRefresh: false, blogKeys: blogKeys);
        }

        private static bool _hasDocumentStoreAnyDocuments = false;

        /// <summary>
        /// Refreshes blogs.
        /// </summary>
        /// <param name="forceRefresh">Sets if the blog should be forced to refresh, ignoring if Blog is up to date.</param>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs.</param>
        public void Refresh(bool forceRefresh = false, params string[] blogKeys) {
            blogKeys = GetAllKeys(blogKeys);

            if(!_hasDocumentStoreAnyDocuments) {
                using(var session = this.Config.DocumentStore.OpenSession()) {
                    _hasDocumentStoreAnyDocuments = session.Query<StoreBlogRefresh>().Any();
                }
            }

            Action<string> refreshFunc = (blogKey) => {
                if(BlogServiceLockHelper.GetIsBlogRefreshing(blogKey)) {
                    return;
                }

                if(!forceRefresh && this.Config.BlogStore.GetIsBlogRefreshed(blogKey, this.Config.CacheTime)) {
                    return;
                }

                var lockObject = BlogServiceLockHelper.GetRefreshedLock(blogKey);
                lock(lockObject) {
                    if(!forceRefresh && this.Config.BlogStore.GetIsBlogRefreshed(blogKey, this.Config.CacheTime)) {
                        return;
                    }

                    this.PerformRefresh(blogKey);
                }
            };

            // First update is always done synchronously
            if(!_hasDocumentStoreAnyDocuments || this.Config.RefreshMode == BlogRefreshMode.Synchronously) {
                Parallel.ForEach(blogKeys, refreshFunc);
            } else {
                Task.Factory.StartNew(() => {
                    Parallel.ForEach(blogKeys, refreshFunc);
                });
            }
            
            // Wait for indexing - if first ever refresh
            if(_hasDocumentStoreAnyDocuments) {
                return;
            }

            this.Config.BlogStore.WaitForIndexes();
        }
        
        private void EnsureBlogsRefreshed(params string[] blogKeys) {
            // If the app uses background-service then don't handle refresh
            if(AppSettingsService.RefreshMode == BlogRefreshMode.BackgroundService) {
                return;
            }

            Refresh(blogKeys);
        }
        
        private void PerformRefresh(string blogKey) {
            try {
                BlogServiceLockHelper.SetIsBlogRefreshing(blogKey);

                // TODO: Change to commented code when Blogger API-bug is fixed:
                // http://code.google.com/p/gdata-issues/issues/detail?id=2555
                //var lastRefresh = _config.BlogStore.GetBlogLastRefresh(key);
                //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastRefresh);

                var bloggerSetting = this.Config.BloggerSettings.First(setting => setting.BlogKey == blogKey);

                var bloggerDocument = BloggerHelper.GetBloggerDocument(bloggerSetting);

                this.Config.BlogStore.Refresh(blogKey, bloggerDocument);
            }
            catch(Exception) {
                throw;
            }
            finally {
                BlogServiceLockHelper.SetIsBlogRefreshing(blogKey, setLocked: false);
            }
        }

        private string GetBlogKeyOrDefault(string blogKey) {
            if(!string.IsNullOrWhiteSpace(blogKey)) {
                return blogKey;
            }

            return this.Config.BloggerSettings.First().BlogKey;
        }

        private string[] GetAllKeys(string[] blogKeys) {
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

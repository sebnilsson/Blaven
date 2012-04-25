using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Configuration;

namespace BloggerViewController {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService {
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

            EnsureBlogsUpdated(this.Config.BloggerSettings.Select(setting => setting.BlogKey).ToArray());
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

        public void Update(params string[] blogKeys) {
            Update(false, blogKeys);
        }

        /// <summary>
        /// Updates blogs.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        public void Update(bool skipCache, params string[] blogKeys) {
            blogKeys = GetAllKeys(blogKeys);
            
            bool hasDatabaseAnyDocuments = (this.Config.DocumentStore.DatabaseCommands.GetStatistics().CountOfDocuments > 0);

            foreach(var blogKey in blogKeys) {
                // TODO: Change to commented code when Blogger API-bug is fixed:
                // http://code.google.com/p/gdata-issues/issues/detail?id=2555
                //var lastUpdate = _config.BlogStore.GetBlogLastUpdate(blogKey);
                //var bloggerDocument = _config.BloggerHelper.GetBloggerDocument(_setting, lastUpdate);

                var bloggerSetting = this.Config.BloggerSettings.First(setting => setting.BlogKey == blogKey);

                var bloggerDocument = this.Config.BloggerHelper.GetBloggerDocument(bloggerSetting);

                this.Config.BlogStore.Update(blogKey, bloggerDocument);
            }

            // Wait for indexing - if first ever update
            if(hasDatabaseAnyDocuments) {
                return;
            }

            while(this.Config.DocumentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0) {
                System.Threading.Thread.Sleep(100);
            }
        }

        private static readonly object _lockStoreLock = new object();
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
        private void EnsureBlogsUpdated(params string[] blogKeys) {
            // If the app uses background-service then don't handle update
            if(AppSettingsService.UseBackgroundService) {
                return;
            }

            foreach(var blogKey in blogKeys) {
                if(this.Config.BlogStore.GetIsBlogUpdated(blogKey, this.Config.CacheTime)) {
                    return;
                }

                var lockObject = GetLock(blogKey);
                lock(lockObject) {
                    if(this.Config.BlogStore.GetIsBlogUpdated(blogKey, this.Config.CacheTime)) {
                        return;
                    }

                    Update(blogKey);
                }
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;
using Raven.Client;
using Raven.Client.Document;

namespace Blaven {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService : IDisposable {
        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features. Uses the default values in AppConfig.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        public BlogService(IDocumentStore documentStore)
            : this(documentStore, new BlogServiceConfig()) {

        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="settings">The Blogger-settings to use.</param>
        public BlogService(IDocumentStore documentStore, IEnumerable<BloggerSetting> settings)
            : this(documentStore, new BlogServiceConfig(settings)) {

        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="config">The Blogger-settings to use in the service.</param>
        public BlogService(IDocumentStore documentStore, BlogServiceConfig config) {
            if(config == null) {
                throw new ArgumentNullException("config");
            }

            this.BlogStore = new RavenDbBlogStore(documentStore);

            this.Config = config;

            this.DocumentStore = documentStore;

            if(this.Config.EnsureBlogsRefreshed) {
                var blogKeys = GetBlogKeysOrAll();
                Refresh(blogKeys);
            }
        }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        internal RavenDbBlogStore BlogStore { get; private set; }

        public IDocumentStore DocumentStore { get; private set; }
        
        public Dictionary<DateTime, int> GetArchiveCount(params string[] blogKeys) {
            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.GetBlogArchiveCount(blogKeys);
        }

        public BlogSelection GetArchiveSelection(DateTime date, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.GetBlogArchiveSelection(date, pageIndex, this.Config.PageSize, blogKeys);
        }
        
        /// <summary>
        /// Gets the BlogInfo for a blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo(string blogKey = null) {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.BlogStore.GetBlogInfo(blogKey);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="permaLink">The perma-link of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string permaLink, string blogKey = null) {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.BlogStore.GetBlogPost(permaLink, blogKey);
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

            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.GetBlogSelection(pageIndex, this.Config.PageSize, blogKeys);
        }

        /// <summary>
        /// Gets the blog-tags count.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        /// <returns>Returns a dictionary of tags and their count.</returns>
        public Dictionary<string, int> GetTagsCount(params string[] blogKeys) {
            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.GetBlogTagsCount(blogKeys);
        }

        public BlogSelection GetTagsSelection(string tagName, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.GetBlogTagsSelection(tagName, pageIndex, this.Config.PageSize, blogKeys);
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex, params string[] blogKeys) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }
            
            blogKeys = GetBlogKeysOrAll(blogKeys);

            return this.BlogStore.SearchPosts(searchTerms ?? string.Empty, pageIndex, this.Config.PageSize, blogKeys);
        }

        /// <summary>
        /// Forces refreshes blogs. Waits for stale indexes and performs update synchronously.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        public IEnumerable<RefreshResult> ForceRefresh(params string[] blogKeys) {
            blogKeys = GetBlogKeysOrAll(blogKeys);

            return PerformRefresh(blogKeys: blogKeys, forceRefresh: true);
        }

        /// <summary>
        /// Refreshes blogs, if needed.
        /// </summary>
        /// <param name="blogKey">The keys of the blogs desired. Leave empty for all blogs</param>
        public IEnumerable<RefreshResult> Refresh(params string[] blogKeys) {
            blogKeys = GetBlogKeysOrAll(blogKeys);

            return PerformRefresh(blogKeys: blogKeys, forceRefresh: false);
        }

        private IEnumerable<RefreshResult> PerformRefresh(IEnumerable<string> blogKeys, bool forceRefresh) {
            var bloggerSettings = this.Config.BloggerSettings.Where(x => blogKeys.Contains(x.BlogKey));

            var updatedBlogs = BlogServiceRefresher.RefreshBlogs(this.BlogStore, bloggerSettings, this.Config.CacheTime, forceRefresh: forceRefresh);

            if(forceRefresh) {
                this.BlogStore.WaitForIndexes();
            }

            return updatedBlogs;
        }

        private string GetBlogKeyOrDefault(string blogKey) {
            if(!string.IsNullOrWhiteSpace(blogKey)) {
                return blogKey;
            }

            return this.Config.BloggerSettings.First().BlogKey;
        }

        internal string[] GetBlogKeysOrAll(string[] blogKeys = null) {
            blogKeys = blogKeys ?? Enumerable.Empty<string>().ToArray();

            if(blogKeys.Any()) {
                return blogKeys;
            }

            return this.Config.BloggerSettings.Select(setting => setting.BlogKey).ToArray();
        }

        /// <summary>
        /// Gets an instance of a DocumentStore, using the values in AppSettings for URL and API-key.
        /// </summary>
        /// <returns></returns>
        public static DocumentStore GetDefaultBlogStore(bool initStore = true) {
            var documentStore = new DocumentStore {
                ApiKey = AppSettingsService.RavenDbStoreApiKey,
                Url = AppSettingsService.RavenDbStoreUrl,
            };

            if(initStore) {
                InitStore(documentStore);
            }

            return documentStore;
        }

        /// <summary>
        /// Initializes the given document store and creates needed indexes for Blaven.
        /// </summary>
        /// <param name="documentStore"></param>
        public static void InitStore(IDocumentStore documentStore) {
            documentStore.Initialize();

            documentStore.Conventions.MaxNumberOfRequestsPerSession = 100;

            var existingIndexes = documentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);
            var blavenIndexes = System.Reflection.Assembly.GetAssembly(typeof(Blaven.BlogService))
                .GetTypes().Where(x => x.IsSubclassOf(typeof(Raven.Client.Indexes.AbstractIndexCreationTask)))
                .Select(x => x.Name);

            var hasAllIndexes = blavenIndexes.All(x => existingIndexes.Contains(x));

            var createIndexesTask = new Task(() => {
                Raven.Client.Indexes.IndexCreation.CreateIndexes(
                    typeof(Blaven.RavenDb.Indexes.BlogPostsOrderedByCreated).Assembly, documentStore);
            });
            createIndexesTask.Start();

            if(!hasAllIndexes) {
                createIndexesTask.Wait();
            }
        }

        #region IDisposable Members

        public void Dispose() {
            if(this.BlogStore != null) {
                this.BlogStore.Dispose();
            }
        }

        #endregion
    }
}

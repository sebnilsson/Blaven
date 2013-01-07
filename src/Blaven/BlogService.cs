using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Blaven.Blogger;
using Blaven.RavenDb;
using Raven.Client;
using Raven.Client.Document;

namespace Blaven
{
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService : IDisposable
    {
        private string[] serviceBlogKeys;

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features. Uses the default values in AppConfig.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="blogKeys">The keys of the blogs desired. Leave empty for all blogs</param>
        public BlogService(IDocumentStore documentStore, params string[] blogKeys)
            : this(documentStore, new BlogServiceConfig(), blogKeys)
        {
        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="settings">The Blogger-settings to use.</param>
        /// <param name="blogKeys">The keys of the blogs desired. Leave empty for all blogs</param>
        public BlogService(IDocumentStore documentStore, IEnumerable<BloggerSetting> settings, params string[] blogKeys)
            : this(documentStore, new BlogServiceConfig(settings), blogKeys)
        {
        }

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to use.</param>
        /// <param name="config">The Blogger-settings to use in the service.</param>
        /// <param name="blogKeys">The keys of the blogs desired. Leave empty for all blogs</param>
        public BlogService(IDocumentStore documentStore, BlogServiceConfig config, params string[] blogKeys)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.DocumentStore = documentStore;

            this.BlogStore = new RavenDbBlogStore(documentStore);

            this.Config = config;

            this.serviceBlogKeys = GetBlogKeysOrAll(blogKeys);

            if (this.Config.EnsureBlogsRefreshed)
            {
                Refresh();
            }
        }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        internal RavenDbBlogStore BlogStore { get; private set; }

        public IDocumentStore DocumentStore { get; private set; }

        public IEnumerable<BlogPostSimple> GetAllBlogPostsSimple()
        {
            return this.BlogStore.GetAllBlogPostsSimple(this.serviceBlogKeys);
        }

        public Dictionary<DateTime, int> GetArchiveCount()
        {
            return this.BlogStore.GetBlogArchiveCount(this.serviceBlogKeys);
        }

        public BlogSelection GetArchiveSelection(DateTime date, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return this.BlogStore.GetBlogArchiveSelection(date, pageIndex, this.Config.PageSize, this.serviceBlogKeys);
        }

        /// <summary>
        /// Gets the BlogInfo for a blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo(string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.BlogStore.GetBlogInfo(blogKey);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="bloggerId">The Blogger ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPostByBloggerId(string bloggerId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.BlogStore.GetBlogPostByBloggerId(blogKey, bloggerId);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="bloggerId">The Blogger ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPostByBloggerId(long bloggerId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            string ravenDbId = RavenDbBlogStore.GetKey<BlogPost>(Convert.ToString(bloggerId));
            return this.BlogStore.GetBlogPostByBloggerId(blogKey, ravenDbId);
        }

        /// <summary>
        /// Gets a blog-post from given ID and blog-key.
        /// </summary>
        /// <param name="blavenId">The ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of Blogger-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string blavenId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            return this.BlogStore.GetBlogPost(blogKey, blavenId);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination. Must have a value of 0 or higher.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetSelection(int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return this.BlogStore.GetBlogSelection(pageIndex, this.Config.PageSize, this.serviceBlogKeys);
        }

        /// <summary>
        /// Gets the blog-tags count.
        /// </summary>
        /// <returns>Returns a dictionary of tags and their count.</returns>
        public Dictionary<string, int> GetTagsCount()
        {
            return this.BlogStore.GetBlogTagsCount(this.serviceBlogKeys);
        }

        public BlogSelection GetTagsSelection(string tagName, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return this.BlogStore.GetBlogTagsSelection(tagName, pageIndex, this.Config.PageSize, this.serviceBlogKeys);
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return this.BlogStore.SearchPosts(searchTerms ?? string.Empty, pageIndex, this.Config.PageSize, this.serviceBlogKeys);
        }

        /// <summary>
        /// Forces refreshes blogs. Waits for stale indexes and performs update synchronously.
        /// </summary>
        public IEnumerable<RefreshResult> ForceRefresh()
        {
            return PerformRefresh(blogKeys: this.serviceBlogKeys, forceRefresh: true);
        }

        /// <summary>
        /// Refreshes blogs, if needed.
        /// </summary>
        public IEnumerable<RefreshResult> Refresh()
        {
            return PerformRefresh(blogKeys: this.serviceBlogKeys, forceRefresh: false);
        }

        private IEnumerable<RefreshResult> PerformRefresh(IEnumerable<string> blogKeys, bool forceRefresh)
        {
            var bloggerSettings = this.Config.BloggerSettings.Where(x => blogKeys.Contains(x.BlogKey));

            var updatedBlogs = BlogServiceRefresher.RefreshBlogs(
                this.BlogStore, bloggerSettings, this.Config.CacheTime, forceRefresh: forceRefresh);

            if (forceRefresh)
            {
                this.BlogStore.WaitForIndexes();
            }

            return updatedBlogs;
        }

        private string GetBlogKeyOrDefault(string blogKey)
        {
            if (!string.IsNullOrWhiteSpace(blogKey))
            {
                return blogKey;
            }

            return this.Config.BloggerSettings.First().BlogKey;
        }

        internal string[] GetBlogKeysOrAll(string[] blogKeys = null)
        {
            blogKeys = blogKeys ?? Enumerable.Empty<string>().ToArray();

            if (blogKeys.Any())
            {
                return blogKeys;
            }

            return this.Config.BloggerSettings.Select(setting => setting.BlogKey).ToArray();
        }

        /// <summary>
        /// Gets an instance of a DocumentStore, using the values in AppSettings for URL and API-key.
        /// </summary>
        /// <returns></returns>
        public static DocumentStore GetDefaultBlogStore(bool initStore = true)
        {
            var documentStore = new DocumentStore
                { ApiKey = AppSettingsService.RavenDbStoreApiKey, Url = AppSettingsService.RavenDbStoreUrl, };

            if (initStore)
            {
                InitStore(documentStore);
            }

            return documentStore;
        }

        /// <summary>
        /// Initializes the given document store and creates needed indexes for Blaven.
        /// </summary>
        /// <param name="documentStore"></param>
        public static void InitStore(IDocumentStore documentStore)
        {
            documentStore.Initialize();

            RegisterListeners(documentStore);

            documentStore.Conventions.MaxNumberOfRequestsPerSession = 100;

            var existingIndexes = documentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);
            IEnumerable<string> blavenIndexes;
            try
            {
                blavenIndexes =
                    Assembly.GetAssembly(typeof(BlogService)).GetTypes().Where(
                        x => x.IsSubclassOf(typeof(Raven.Client.Indexes.AbstractIndexCreationTask))).Select(x => x.Name);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var firstError = ex.LoaderExceptions.FirstOrDefault();
                string message = (firstError != null) ? firstError.Message : string.Empty;

                throw new BlavenException(message, ex);
            }
            var hasAllIndexes = blavenIndexes.All(x => existingIndexes.Contains(x));

            var createIndexesTask =
                new Task(
                    () => Raven.Client.Indexes.IndexCreation.CreateIndexes(
                        typeof(Blaven.RavenDb.Indexes.BlogPostsOrderedByCreated).Assembly, documentStore));
            createIndexesTask.Start();

            if (!hasAllIndexes)
            {
                createIndexesTask.Wait();
            }
        }

        private static void RegisterListeners(IDocumentStore documentStore)
        {
            var store = documentStore as DocumentStoreBase;
            if (store == null)
            {
                return;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.BlogStore != null)
            {
                this.BlogStore.Dispose();
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.DataSources;
using Blaven.RavenDb;
using Blaven.Transformers;
using Raven.Client;

namespace Blaven
{
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService : IDisposable
    {
        private readonly string[] blogKeysFilter;

        private readonly DataSourceRefreshService dataSourceRefresher;

        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="documentStore">The DocumentStore to store blog-posts in.</param>
        /// <param name="config">The config to use in the service.</param>
        /// <param name="settings">The blog-settings the service will use. Will be filtered by "blogKeysFilter".</param>
        /// <param name="blogPostTransformers">The transformers to apply to blog-posts.</param>
        /// <param name="blogKeysFilter">A list of keys to filter the service on. Leave empty for all blogs.</param>
        public BlogService(
            IDocumentStore documentStore,
            BlogServiceConfig config = null,
            IEnumerable<BlavenBlogSetting> settings = null,
            BlogPostTransformersCollection blogPostTransformers = null,
            IEnumerable<string> blogKeysFilter = null)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException("documentStore");
            }

            this.DocumentStore = documentStore;
            this.Repository = new Repository(this.DocumentStore);

            this.Config = config ?? new BlogServiceConfig();
            this.Settings = (settings ?? BlavenBlogSettingsParser.ParseFile()).ToList();
            this.BlogPostTransformers = blogPostTransformers ?? BlogPostTransformersCollection.Default;

            var blogKeysFilterItems = (blogKeysFilter ?? Enumerable.Empty<string>()).ToArray();
            if (!blogKeysFilterItems.Any())
            {
                blogKeysFilterItems = this.Settings.Select(x => x.BlogKey).ToArray();
            }
            else
            {
                if (this.Settings.Any(x => !blogKeysFilterItems.Contains(x.BlogKey)))
                {
                    throw new ArgumentOutOfRangeException(
                        "settings", "One or more blogKeys are not present in provided settings.");
                }
                this.Settings = this.Settings.Where(x => blogKeysFilterItems.Contains(x.BlogKey));
            }

            this.blogKeysFilter = blogKeysFilterItems.ToArray();

            this.dataSourceRefresher = new DataSourceRefreshService(this.Config, this.Repository);

            if (this.Config.EnsureBlogsRefreshed)
            {
                Refresh();
            }
        }

        internal Repository Repository { get; private set; }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        public IDocumentStore DocumentStore { get; private set; }

        public BlogPostTransformersCollection BlogPostTransformers { get; private set; }

        public IEnumerable<BlavenBlogSetting> Settings { get; private set; }

        public IEnumerable<BlogPostHead> GetAllPostHeads()
        {
            return this.Repository.GetAllBlogPostHeads(this.blogKeysFilter);
        }

        public Dictionary<DateTime, int> GetArchiveCount()
        {
            return this.Repository.GetBlogArchiveCount(this.blogKeysFilter);
        }

        public BlogPostCollection GetArchivePosts(DateTime date, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return
                this.Repository.GetBlogArchiveSelection(date, pageIndex, this.Config.PageSize, this.blogKeysFilter)
                    .ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Gets the BlogInfo for a blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of blog-settings.</param>
        /// <returns>Returns information about a blog.</returns>
        public BlogInfo GetInfo(string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            var blogInfo = this.Repository.GetBlogInfo(blogKey);
            if (blogInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                    "blogKey", string.Format("No blog-info was found for blog-key '{0}'.", blogKey));
            }

            return blogInfo;
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="dataSourceId">The data source-ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of blog-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPostByDataSourceId(string dataSourceId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            string blavenHash = BlavenHelper.GetBlavenHash(dataSourceId);
            string ravenDbId = RavenDbHelper.GetEntityId<BlogPost>(blavenHash);

            return this.Repository.GetBlogPost(blogKey, ravenDbId).ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Gets a blog-post from given perma-link and blog-key.
        /// </summary>
        /// <param name="dataSourceId"> data source-ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of blog-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPostByDataSourceId(ulong dataSourceId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            string blavenHash = BlavenHelper.GetBlavenHash(dataSourceId);
            string ravenDbId = RavenDbHelper.GetEntityId<BlogPost>(blavenHash);

            return this.Repository.GetBlogPost(blogKey, ravenDbId).ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Gets a blog-post from given ID and blog-key.
        /// </summary>
        /// <param name="blavenId">The ID of the blog-post to get.</param>
        /// <param name="blogKey">The key of the blog desired. Defaults to first blog in the collection of blog-settings.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetPost(string blavenId, string blogKey = null)
        {
            blogKey = GetBlogKeyOrDefault(blogKey);

            string ravenDbId = RavenDbHelper.GetEntityId<BlogPost>(blavenId);

            return this.Repository.GetBlogPost(blogKey, ravenDbId).ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination. Must have a value of 0 or higher.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogPostCollection GetPosts(int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return
                this.Repository.GetBlogSelection(pageIndex, this.Config.PageSize, this.blogKeysFilter)
                    .ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Gets the blog-tags count.
        /// </summary>
        /// <returns>Returns a dictionary of tags and their count.</returns>
        public Dictionary<string, int> GetTagsCount()
        {
            return this.Repository.GetBlogTagsCount(this.blogKeysFilter);
        }

        public BlogPostCollection GetTagPosts(string tagName, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return
                this.Repository.GetBlogTagsSelection(tagName, pageIndex, this.Config.PageSize, this.blogKeysFilter)
                    .ApplyTransformers(this.BlogPostTransformers);
        }

        public BlogPostCollection SearchPosts(string searchTerms, int pageIndex)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            return
                this.Repository.GetBlogPostsSearch(
                    searchTerms ?? string.Empty, pageIndex, this.Config.PageSize, this.blogKeysFilter)
                    .ApplyTransformers(this.BlogPostTransformers);
        }

        /// <summary>
        /// Refreshes blogs, if needed.
        /// <param name="forceRefresh">Sets if the refresh should be forced. Defaults to "false".</param>
        /// </summary>
        public IEnumerable<RefreshSynchronizerResult> Refresh(bool forceRefresh = false)
        {
            var updatedBlogs = dataSourceRefresher.Refresh(this.Settings, forceRefresh);

            if (forceRefresh)
            {
                this.Repository.WaitForPosts(this.Settings.Select(x => x.BlogKey).ToArray());
            }

            return updatedBlogs;
        }

        public BlavenBlogSetting GetSetting(string blogKey)
        {
            var setting = this.Settings.FirstOrDefault(x => x.BlogKey == blogKey);
            if (setting == null)
            {
                throw new KeyNotFoundException(blogKey);
            }
            return setting;
        }

        public BlogService Clone(BlogServiceConfig config = null)
        {
            return new BlogService(
                this.DocumentStore, config, this.Settings, this.BlogPostTransformers, this.blogKeysFilter);
        }

        private string GetBlogKeyOrDefault(string blogKey)
        {
            return !string.IsNullOrWhiteSpace(blogKey) ? blogKey : this.GetBlogKeysOrAll().First();
        }

        private IEnumerable<string> GetBlogKeysOrAll(string[] blogKeys = null)
        {
            blogKeys = blogKeys ?? Enumerable.Empty<string>().ToArray();

            return blogKeys.Any() ? blogKeys : this.Settings.Select(x => x.BlogKey).ToArray();
        }

        public static void InitInstance(
            IDocumentStore documentStore = null,
            BlogServiceConfig config = null,
            IEnumerable<BlavenBlogSetting> settings = null,
            BlogPostTransformersCollection blogPostTransformers = null,
            IEnumerable<string> blogKeys = null)
        {
            instanceLazy =
                new RequestLazy<BlogService>(
                    () => new BlogService(documentStore, config, settings, blogPostTransformers, blogKeys));
        }

        private static RequestLazy<BlogService> instanceLazy;

        public static BlogService Instance
        {
            get
            {
                if (instanceLazy == null)
                {
                    throw new BlavenNotInitException();
                }
                return instanceLazy.Value;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.Repository != null)
            {
                this.Repository.Dispose();
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.DataSources;
using Blaven.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.RavenDb
{
    internal class RavenRepository : IDisposable
    {
        private const int WaitForDataTimeoutSeconds = 15;

        private const int WaitForIndexTimeoutSeconds = 30;

        private readonly IDocumentStore documentStore;

        private readonly Lazy<IDocumentSession> lazyQuerySession;

        public RavenRepository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;

            this.lazyQuerySession = new Lazy<IDocumentSession>(() => this.documentStore.OpenSession());
        }

        private IDocumentSession QuerySession
        {
            get
            {
                return this.lazyQuerySession.Value;
            }
        }

        public IEnumerable<BlogPostHead> GetAllBlogPostHeads(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            using (var maximumSession = GetMaximumRequestSession())
            {
                var allPosts =
                    maximumSession.Query<BlogPost, BlogPostHeads>()
                                  .Where(x => x.BlogKey.In(blogKeys))
                                  .Select(
                                      x =>
                                      new BlogPostHead
                                          {
                                              BlavenId = x.BlavenId,
                                              BlogKey = x.BlogKey,
                                              Published = x.Published,
                                              Tags = x.Tags,
                                              Title = x.Title,
                                              Updated = x.Updated,
                                              UrlSlug = x.UrlSlug,
                                          })
                                  .OrderByDescending(x => x.Published)
                                  .Take(int.MaxValue);

                return RavenDbHelper.HandleRavenExceptions(() => allPosts.ToList());
            }
        }

        public List<BlogPostMeta> GetAllBlogPostMeta(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            using (var maximumSession = GetMaximumRequestSession())
            {
                var postMetas =
                    maximumSession.Query<BlogPost, BlogPostMetas>()
                                  .Where(x => x.BlogKey.In(blogKeys))
                                  .Select(
                                      x =>
                                      new BlogPostMeta
                                          {
                                              BlogKey = x.BlogKey,
                                              Checksum = x.Checksum,
                                              DataSourceId = x.DataSourceId,
                                              Id = x.Id,
                                              Published = x.Published,
                                          })
                                  .Take(int.MaxValue);

                return RavenDbHelper.HandleRavenExceptions(() => postMetas.ToList());
            }
        }

        public Dictionary<DateTime, int> GetBlogArchiveCount(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var archiveCount =
                this.QuerySession.Query<ArchiveCountByBlogKey.ReduceResult, ArchiveCountByBlogKey>()
                    .Where(x => x.BlogKey.In(blogKeys));
            var groupedArchive = from archive in archiveCount.ToList()
                                 group archive by archive.Date
                                 into g select new { Date = g.Key, Count = g.Sum(x => x.Count) };

            return
                RavenDbHelper.HandleRavenExceptions(
                    () => groupedArchive.OrderByDescending(x => x.Date).ToDictionary(x => x.Date, x => x.Count));
        }

        public BlogPostCollection GetBlogArchiveSelection(DateTime date, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                this.QuerySession.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(
                        x => x.BlogKey.In(blogKeys) && x.Published.Year == date.Year && x.Published.Month == date.Month)
                    .OrderByDescending(x => x.Published);

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public BlogInfo GetBlogInfo(string blogKey)
        {
            string blogInfoId = RavenDbHelper.GetEntityId<BlogInfo>(blogKey);
            var blogInfo = this.QuerySession.Load<BlogInfo>(blogInfoId);

            if (blogInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                    "blogKey", string.Format("No blog-info was found for blog-key '{0}'.", blogKey));
            }

            return blogInfo;
        }

        public bool GetIsBlogRefreshed(string blogKey, int cacheTimeMinutes)
        {
            var lastRefresh = GetBlogRefreshTimestamp(blogKey);
            if (!lastRefresh.HasValue)
            {
                return false;
            }

            return lastRefresh.Value.AddMinutes(cacheTimeMinutes) > DateTime.Now;
        }

        public DateTime? GetBlogRefreshTimestamp(string blogKey)
        {
            string blogRefreshId = RavenDbHelper.GetEntityId<BlogRefresh>(blogKey);
            var blogRefresh = this.QuerySession.Load<BlogRefresh>(blogRefreshId);

            return (blogRefresh != null) ? blogRefresh.Timestamp : (DateTime?)null;
        }

        public BlogPost GetBlogPost(string blogKey, string ravenId)
        {
            var post = this.QuerySession.Load<BlogPost>(ravenId);
            return (post != null && post.BlogKey == blogKey) ? post : null;
        }

        public BlogPostCollection GetBlogSelection(int pageIndex, int pageSize, params string[] blogKeys)
        {
            if (blogKeys == null || !blogKeys.Any())
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                this.QuerySession.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(x => x.BlogKey.In(blogKeys))
                    .OrderByDescending(x => x.Published);

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public Dictionary<string, int> GetBlogTagsCount(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var tagCount =
                this.QuerySession.Query<TagsCountByBlogKey.ReduceResult, TagsCountByBlogKey>()
                    .Where(x => x.BlogKey.In(blogKeys));
            var tagsGrouped = (from result in tagCount.ToList()
                               group result by result.Tag
                               into g select new { Tag = g.Key, Count = g.Sum(x => x.Count), });

            return
                RavenDbHelper.HandleRavenExceptions(
                    () => tagsGrouped.OrderBy(x => x.Tag).ToDictionary(x => x.Tag, x => x.Count));
        }

        public BlogPostCollection GetBlogTagsSelection(string tagName, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                this.QuerySession.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(
                        x =>
                        x.BlogKey.In(blogKeys)
                        && x.Tags.Any(tag => tag.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
                    .OrderByDescending(x => x.Published);

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public bool GetHasBlogAnyData(string blogKey)
        {
            using (var maximumSession = this.GetMaximumRequestSession())
            {
                return maximumSession.Query<BlogRefresh>().Any(x => x.BlogKey == blogKey);
            }
        }

        public void Refresh(string blogKey, BlogData blogData, bool throwOnCritical = false)
        {
            var refreshResult = new DataSourceRefreshResult
                                    {
                                        BlogInfo = blogData.Info,
                                        ModifiedBlogPosts = blogData.Posts
                                    };
            Refresh(blogKey, refreshResult, throwOnCritical);
        }

        public void Refresh(string blogKey, DataSourceRefreshResult refreshResult, bool throwOnException = false)
        {
            if (refreshResult == null)
            {
                throw new ArgumentNullException("refreshResult");
            }

            refreshResult.ModifiedBlogPosts = refreshResult.ModifiedBlogPosts ?? Enumerable.Empty<BlogPost>();
            refreshResult.RemovedBlogPostIds = refreshResult.RemovedBlogPostIds ?? Enumerable.Empty<string>();

            try
            {
                using (var refreshSession = GetMaximumRequestSession())
                {
                    refreshResult.ModifiedBlogPosts = RemoveDuplicatePosts(
                        blogKey, refreshResult.ModifiedBlogPosts, throwOnException);

                    RefreshBlogInfo(refreshSession, blogKey, refreshResult.BlogInfo);

                    RefreshBlogPosts(refreshSession, refreshResult.ModifiedBlogPosts);

                    FlagDeletedBlogPosts(refreshSession, refreshResult.RemovedBlogPostIds);

                    UpdateBlogRefresh(refreshSession, blogKey);

                    refreshSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new RavenRepositoryRefreshException(blogKey, ex);
            }
        }

        public BlogPostCollection SearchPosts(string searchTerms, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            string where = string.Format("Content:\"{0}\"", searchTerms);
            var posts = this.QuerySession.Advanced.LuceneQuery<BlogPost, SearchBlogPosts>().Where(where);

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public void WaitForStaleIndexes()
        {
            var waitTask = new Task(
                () =>
                    {
                        while (this.documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                        {
                            Thread.Sleep(100);
                        }
                    });
            waitTask.Start();
            waitTask.Wait(TimeSpan.FromSeconds(WaitForIndexTimeoutSeconds));
        }

        public void WaitForData(string blogKey)
        {
            var waitTask = new Task(
                () =>
                    {
                        while (!this.GetHasBlogAnyData(blogKey))
                        {
                            Thread.Sleep(200);
                        }
                    });

            waitTask.Start();
            waitTask.Wait(TimeSpan.FromSeconds(WaitForDataTimeoutSeconds));
        }

        private IDocumentSession GetMaximumRequestSession()
        {
            var maximumSession = this.documentStore.OpenSession();
            maximumSession.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
            return maximumSession;
        }

        private IEnumerable<BlogPost> RemoveDuplicatePosts(
            string blogKey, IEnumerable<BlogPost> modifiedPosts, bool throwOnException)
        {
            var postMeta = this.GetAllBlogPostMeta(blogKey);
            var duplicateItems = (from modified in modifiedPosts
                                  where
                                      postMeta.Any(x => x.Id == modified.Id && x.DataSourceId != modified.DataSourceId)
                                  select modified).ToList();

            if (!duplicateItems.Any())
            {
                return modifiedPosts;
            }

            if (throwOnException)
            {
                var duplicatePost = modifiedPosts.First();
                string exceptionMessage =
                    string.Format(
                        "Duplicate calculated ID by Blaven for post with title '{0}'."
                        + " Move content of post to a new item to get a new calculated ID.",
                        duplicatePost.Title);
                throw new BlavenBlogException(blogKey, message: exceptionMessage);
            }

            var distinctModifiedPosts = modifiedPosts.ToList();
            duplicateItems.ForEach(x => distinctModifiedPosts.Remove(x));

            return distinctModifiedPosts;
        }

        private static void RefreshBlogInfo(IDocumentSession session, string blogKey, BlogInfo updateInfo)
        {
            if (updateInfo == null)
            {
                return;
            }

            string blogInfoUrl = RavenDbHelper.GetEntityId<BlogInfo>(blogKey);
            var blogInfo = session.Load<BlogInfo>(blogInfoUrl);

            if (blogInfo == null)
            {
                blogInfo = new BlogInfo { BlogKey = blogKey };
                session.Store(blogInfo, blogInfoUrl);
            }

            blogInfo.Subtitle = updateInfo.Subtitle;
            blogInfo.Title = updateInfo.Title;
            blogInfo.Updated = updateInfo.Updated;
            blogInfo.Url = updateInfo.Url;
        }

        private static void RefreshBlogPosts(IDocumentSession session, IEnumerable<BlogPost> modifiedPosts)
        {
            var posts = modifiedPosts.ToList();
            if (!posts.Any())
            {
                return;
            }

            var updatedPostsList = posts.OrderBy(x => x.Id).ToList();
            var updatedPostsIds = updatedPostsList.Select(GetPostRavenId).ToList();

            var storedPosts = session.Load<BlogPost>(updatedPostsIds);
            for (int i = 0; i < storedPosts.Length; i++)
            {
                var storedPost = storedPosts[i];
                var updatedPost = updatedPostsList[i];

                if (storedPost == null)
                {
                    storedPost = updatedPost;
                    session.Store(storedPost);
                }
                else
                {
                    storedPost.Author = updatedPost.Author;
                    storedPost.BlavenId = updatedPost.BlavenId;
                    storedPost.BlogKey = updatedPost.BlogKey;
                    storedPost.Checksum = updatedPost.Checksum;
                    storedPost.Content = updatedPost.Content;
                    storedPost.DataSourceId = updatedPost.DataSourceId;
                    storedPost.DataSourceUrl = updatedPost.DataSourceUrl;
                    storedPost.IsDeleted = updatedPost.IsDeleted;
                    storedPost.Published = updatedPost.Published;
                    storedPost.Tags = updatedPost.Tags;
                    storedPost.Title = updatedPost.Title;
                    storedPost.Updated = updatedPost.Updated;
                    storedPost.UrlSlug = updatedPost.UrlSlug;
                }
            }
        }

        private static string GetPostRavenId(BlogPost blogPost)
        {
            return !string.IsNullOrWhiteSpace(blogPost.Id)
                       ? blogPost.Id
                       : BlavenHelper.GetBlavenHash(blogPost.DataSourceId);
        }

        private static void UpdateBlogRefresh(IDocumentSession session, string blogKey)
        {
            string blogRefreshId = RavenDbHelper.GetEntityId<BlogRefresh>(blogKey);
            var blogRefresh = session.Load<BlogRefresh>(blogRefreshId);

            if (blogRefresh == null)
            {
                blogRefresh = new BlogRefresh { BlogKey = blogKey };
                session.Store(blogRefresh, blogRefreshId);
            }
            blogRefresh.Timestamp = DateTime.Now;
        }

        private static void FlagDeletedBlogPosts(IDocumentSession session, IEnumerable<string> blogPostIds)
        {
            var deletedPosts = session.Load<BlogPost>(blogPostIds);
            foreach (var deletedPost in deletedPosts)
            {
                deletedPost.IsDeleted = true;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.lazyQuerySession.IsValueCreated && this.lazyQuerySession.Value != null)
            {
                this.lazyQuerySession.Value.Dispose();
            }
        }

        #endregion
    }
}
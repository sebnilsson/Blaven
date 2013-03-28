using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.RavenDb
{
    internal class RavenDbBlogStore : IDisposable
    {
        private const int waitForIndexTimeout = 1000 * 30;

        private IDocumentStore _documentStore;

        public RavenDbBlogStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;

            _lazySession = new Lazy<IDocumentSession>(() => { return _documentStore.OpenSession(); });
        }

        private Lazy<IDocumentSession> _lazySession;

        private IDocumentSession _session
        {
            get
            {
                return _lazySession.Value;
            }
        }

        public IEnumerable<BlogPostSimple> GetAllBlogPostsSimple(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            using (var session = _documentStore.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;
                var allPosts =
                    session.Query<BlogPost, BlogPostsSimple>()
                           .Select(
                               x =>
                               new BlogPostSimple
                                   {
                                       BlavenId = x.BlavenId,
                                       BlogKey = x.BlogKey,
                                       Published = x.Published,
                                       Tags = x.Tags,
                                       Title = x.Title,
                                       Updated = x.Updated,
                                       UrlSlug = x.UrlSlug,
                                   });
                try
                {
                    return allPosts.OrderByDescending(x => x.Published).ToList();
                }
                catch (Exception ex)
                {
                    if (ex.Source == "Raven.Database")
                    {
                        throw new BlogServiceNotInitException(ex);
                    }
                    throw;
                }
            }
        }

        public Dictionary<DateTime, int> GetBlogArchiveCount(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var archiveCount =
                _session.Query<ArchiveCountByBlogKey.ReduceResult, ArchiveCountByBlogKey>()
                        .Where(x => x.BlogKey.In(blogKeys));
            var groupedArchive = from archive in archiveCount.ToList()
                                 group archive by archive.Date
                                 into g select new { Date = g.Key, Count = g.Sum(x => x.Count) };

            try
            {
                return groupedArchive.OrderByDescending(x => x.Date).ToDictionary(x => x.Date, x => x.Count);
            }
            catch (Exception ex)
            {
                if (ex.Source == "Raven.Database")
                {
                    throw new BlogServiceNotInitException(ex);
                }
                throw;
            }
        }

        public BlogSelection GetBlogArchiveSelection(DateTime date, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                _session.Query<BlogPost, BlogPostsOrderedByCreated>()
                        .Where(
                            x =>
                            x.BlogKey.In(blogKeys) && x.Published.Year == date.Year && x.Published.Month == date.Month)
                        .OrderByDescending(x => x.Published);

            return new BlogSelection(posts, pageIndex, pageSize);
        }

        public BlogInfo GetBlogInfo(string blogKey)
        {
            var blogInfo = _session.Load<BlogInfo>(GetKey<BlogInfo>(blogKey));
            if (blogInfo == null)
            {
                throw new ArgumentOutOfRangeException(
                    "blogKey", string.Format("No blog-info was found for blog-key '{0}'.", blogKey));
            }

            return blogInfo;
        }

        public bool GetIsBlogRefreshed(string blogKey, int cacheTimeMinutes)
        {
            var lastRefresh = GetBlogLastRefresh(blogKey);
            if (!lastRefresh.HasValue)
            {
                return false;
            }

            return lastRefresh.Value.AddMinutes(cacheTimeMinutes) > DateTime.Now;
        }

        public DateTime? GetBlogLastRefresh(string blogKey)
        {
            var storeRefresh = _session.Load<StoreBlogRefresh>(GetKey<StoreBlogRefresh>(blogKey));
            if (storeRefresh == null)
            {
                return null;
            }

            return storeRefresh.Updated;
        }

        public BlogPost GetBlogPostByBloggerId(string blogKey, string bloggerId)
        {
            var blogPost =
                _session.Query<BlogPost>().FirstOrDefault(post => post.BlogKey == blogKey && post.Id == bloggerId);
            return blogPost;
        }

        public BlogPost GetBlogPost(string blogKey, string blavenId)
        {
            var blogPost =
                _session.Query<BlogPost>().FirstOrDefault(post => post.BlogKey == blogKey && post.BlavenId == blavenId);
            return blogPost;
        }

        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, params string[] blogKeys)
        {
            if (blogKeys == null || !blogKeys.Any())
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                _session.Query<BlogPost, BlogPostsOrderedByCreated>()
                        .Where(x => x.BlogKey.In(blogKeys))
                        .OrderByDescending(x => x.Published);

            return new BlogSelection(posts, pageIndex, pageSize);
        }

        public Dictionary<string, int> GetBlogTagsCount(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var tagCount =
                _session.Query<TagsCountByBlogKey.ReduceResult, TagsCountByBlogKey>().Where(x => x.BlogKey.In(blogKeys));
            var tagsGrouped = (from result in tagCount.ToList()
                               group result by result.Tag
                               into g select new { Tag = g.Key, Count = g.Sum(x => x.Count), });
            try
            {
                return tagsGrouped.OrderBy(x => x.Tag).ToDictionary(x => x.Tag, x => x.Count);
            }
            catch (Exception ex)
            {
                if (ex.Source == "Raven.Database")
                {
                    throw new BlogServiceNotInitException(ex);
                }
                throw;
            }
        }

        public BlogSelection GetBlogTagsSelection(string tagName, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            var posts =
                _session.Query<BlogPost, BlogPostsOrderedByCreated>()
                        .Where(
                            x =>
                            x.BlogKey.In(blogKeys)
                            && x.Tags.Any(tag => tag.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
                        .OrderByDescending(x => x.Published);

            return new BlogSelection(posts, pageIndex, pageSize);
        }

        public bool GetHasBlogAnyData(string blogKey)
        {
            using (var session = _documentStore.OpenSession())
            {
                return session.Query<StoreBlogRefresh>().Any(x => x.BlogKey == blogKey);
            }
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            string where = string.Format("Content:\"{0}\"", searchTerms);
            var posts = _session.Advanced.LuceneQuery<BlogPost, SearchBlogPosts>().Where(where);

            return new BlogSelection(posts, pageIndex, pageSize);
        }

        public void Refresh(string blogKey, BlogData parsedBlogData, bool waitForIndexes = false)
        {
            RefreshBlogInfo(_session, blogKey, parsedBlogData);

            RefreshBlogPosts(_session, blogKey, parsedBlogData.Posts);

            UpdateStoreRefresh(_session, blogKey);

            _session.SaveChanges();

            if (waitForIndexes)
            {
                WaitForIndexes();
            }
        }

        public void WaitForIndexes()
        {
            var waitTask = new Task(
                () =>
                    {
                        while (_documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    });
            waitTask.Start();
            waitTask.Wait(waitForIndexTimeout);
        }

        private void RefreshBlogInfo(IDocumentSession session, string blogKey, BlogData parsedData)
        {
            string blogInfoUrl = GetKey<BlogInfo>(blogKey);
            var blogInfo = session.Load<BlogInfo>(blogInfoUrl);

            if (blogInfo == null)
            {
                blogInfo = new BlogInfo { BlogKey = blogKey };
                session.Store(blogInfo, blogInfoUrl);
            }

            blogInfo.Subtitle = parsedData.Info.Subtitle;
            blogInfo.Title = parsedData.Info.Title;
            blogInfo.Updated = parsedData.Info.Updated;
            blogInfo.Url = parsedData.Info.Url;
        }

        private void RefreshBlogPosts(IDocumentSession session, string blogKey, IEnumerable<BlogPost> parsedPosts)
        {
            var parsedPostsList = parsedPosts.OrderBy(x => x.Id).ToList();

            var parsedPostsIds = parsedPosts.Select(x => x.Id).OrderBy(x => x).ToList();
            var storePosts = session.Load<BlogPost>(parsedPostsIds);

            for (int i = 0; i < storePosts.Count(); i++)
            {
                var parsedPost = parsedPostsList[i];

                var storePost = storePosts[i];
                if (storePost == null)
                {
                    storePost = parsedPost;

                    session.Store(storePost);
                }
                else
                {
                    storePost.Author = parsedPost.Author;
                    storePost.Content = parsedPost.Content;
                    storePost.OriginalBloggerUrl = parsedPost.OriginalBloggerUrl;
                    storePost.Published = parsedPost.Published;
                    storePost.Tags = parsedPost.Tags;
                    storePost.Title = parsedPost.Title;
                    storePost.Updated = parsedPost.Updated;
                    storePost.UrlSlug = parsedPost.UrlSlug;

                    storePost.IsDeleted = false;
                }
            }

            var storePostsIds =
                session.Query<BlogPost>().Where(x => x.BlogKey == blogKey).Select(x => x.Id).Take(int.MaxValue).ToList();
            var removedPostIds = storePostsIds.Where(postId => !parsedPostsIds.Contains(postId));

            var removedPosts = session.Load<BlogPost>(removedPostIds);
            foreach (var removedPost in removedPosts)
            {
                removedPost.IsDeleted = true;
            }
        }

        internal void UpdateStoreRefresh(string blogKey)
        {
            UpdateStoreRefresh(_session, blogKey);

            _session.SaveChanges();
        }

        private void UpdateStoreRefresh(IDocumentSession session, string blogKey)
        {
            string storeUpdateUrl = GetKey<StoreBlogRefresh>(blogKey);
            var storeUpdate = session.Load<StoreBlogRefresh>(storeUpdateUrl);

            if (storeUpdate == null)
            {
                storeUpdate = new StoreBlogRefresh { BlogKey = blogKey };
                session.Store(storeUpdate, storeUpdateUrl);
            }
            storeUpdate.Updated = DateTime.Now;
        }

        public static string GetKey<T>(params string[] keys)
        {
            if (keys == null || !keys.Any())
            {
                throw new ArgumentOutOfRangeException("keys", "The provided keys cannot be null or empty.");
            }

            string typeName = typeof(T).Name.ToLowerInvariant();
            string key = string.Join("/", new[] { typeName }.Concat(keys ?? Enumerable.Empty<string>()));
            return key;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_lazySession.IsValueCreated)
            {
                _lazySession.Value.Dispose();
            }
        }

        #endregion
    }
}
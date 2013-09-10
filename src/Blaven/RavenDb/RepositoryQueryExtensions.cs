using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb.Indexes;
using Raven.Client.Linq;

namespace Blaven.RavenDb
{
    internal static class RepositoryQueryExtensions
    {
        public static IList<BlogPostHead> GetAllBlogPostHeads(this Repository repository, params string[] blogKeys)
        {
            var allPosts =
                repository.GetMaxRequestQuery<BlogPostHeads>(blogKeys)
                          .Select(x => new BlogPostHead(x))
                          .OrderByDescending(x => x.Published)
                          .Take(int.MaxValue);

            return allPosts.ToListHandled();
        }

        public static IList<BlogPostMeta> GetAllBlogPostMeta(this Repository repository, params string[] blogKeys)
        {
            var postMetas =
                repository.GetMaxRequestQuery<BlogPostMetas>(blogKeys)
                          .Select(x => new BlogPostMeta(x))
                          .OrderByDescending(x => x.Published)
                          .Take(int.MaxValue);

            return postMetas.ToListHandled();
        }

        public static Dictionary<DateTime, int> GetBlogArchiveCount(
            this Repository repository, params string[] blogKeys)
        {
            var archiveCount =
                repository.GetQuery<ArchiveCountByBlogKey.ReduceResult, ArchiveCountByBlogKey>()
                          .Where(x => x.BlogKey.In(blogKeys));

            var groupedArchive = from archive in archiveCount.ToList()
                                 group archive by archive.Date
                                 into g select new { Date = g.Key, Count = g.Sum(x => x.Count) };

            return
                RavenDbHelper.HandleRavenExceptions(
                    () => groupedArchive.OrderByDescending(x => x.Date).ToDictionary(x => x.Date, x => x.Count));
        }

        public static BlogPostCollection GetBlogArchiveSelection(
            this Repository repository, DateTime date, int pageIndex, int pageSize, string[] blogKeys)
        {
            var posts =
                repository.GetBlogPostQuery()
                          .Where(x => x.Published.Year == date.Year && x.Published.Month == date.Month);

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public static BlogInfo GetBlogInfo(this Repository repository, string blogKey)
        {
            string blogInfoId = RavenDbHelper.GetEntityId<BlogInfo>(blogKey);
            var blogInfo = repository.CurrentSession.Load<BlogInfo>(blogInfoId);

            return blogInfo;
        }

        public static BlogPost GetBlogPost(this Repository repository, string blogKey, string ravenId)
        {
            var post = repository.CurrentSession.Load<BlogPost>(ravenId);
            return (post != null && post.BlogKey == blogKey) ? post : null;
        }

        public static BlogPostCollection GetBlogPostsSearch(
            this Repository repository, string searchTerms, int pageIndex, int pageSize, string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            string where = string.Format("Content:\"{0}\"", searchTerms);
            var posts =
                repository.CurrentSession.Advanced.LuceneQuery<BlogPost, SearchBlogPosts>().Where(where).AsQueryable();

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public static bool GetIsBlogRefreshed(this Repository repository, string blogKey, int cacheTimeMinutes)
        {
            var lastRefresh = GetBlogRefreshTimestamp(repository, blogKey);
            if (!lastRefresh.HasValue)
            {
                return false;
            }

            return lastRefresh.Value.AddMinutes(cacheTimeMinutes) > DateTime.Now;
        }

        public static DateTime? GetBlogRefreshTimestamp(this Repository repository, string blogKey)
        {
            string ravenId = RavenDbHelper.GetEntityId<BlogRefresh>(blogKey);
            var blogRefresh = repository.CurrentSession.Load<BlogRefresh>(ravenId);

            return (blogRefresh != null) ? blogRefresh.Timestamp : (DateTime?)null;
        }

        public static BlogPostCollection GetBlogSelection(
            this Repository repository, int pageIndex, int pageSize, params string[] blogKeys)
        {
            var posts = repository.GetBlogPostQuery();
            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public static Dictionary<string, int> GetBlogTagsCount(this Repository repository, params string[] blogKeys)
        {
            var tagCount =
                repository.GetQuery<TagsCountByBlogKey.ReduceResult, TagsCountByBlogKey>()
                          .Where(x => x.BlogKey.In(blogKeys));
            var tagsGrouped = (from result in tagCount.ToList()
                               group result by result.Tag
                               into g select new { Tag = g.Key, Count = g.Sum(x => x.Count), });

            return
                RavenDbHelper.HandleRavenExceptions(
                    () => tagsGrouped.OrderBy(x => x.Tag).ToDictionary(x => x.Tag, x => x.Count));
        }

        public static BlogPostCollection GetBlogTagsSelection(
            this Repository repository, string tagName, int pageIndex, int pageSize, string[] blogKeys)
        {
            var posts =
                repository.GetBlogPostQuery()
                          .Where(
                              x => x.Tags.Any(tag => tag.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)));

            return new BlogPostCollection(posts, pageIndex, pageSize);
        }

        public static bool GetHasBlogAnyData(this Repository repository, string blogKey)
        {
            string ravenId = RavenDbHelper.GetEntityId<BlogRefresh>(blogKey);
            var blogRefresh = repository.CurrentSession.Load<BlogRefresh>(ravenId);

            return (blogRefresh != null);
        }
    }
}
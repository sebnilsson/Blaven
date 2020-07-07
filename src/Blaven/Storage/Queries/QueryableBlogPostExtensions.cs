using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Storage.Queries
{
    public static class QueryableBlogPostExtensions
    {
        public static IQueryable<BlogPost> ApplyOptions(
            this IQueryable<BlogPost> queryable,
            BlogQueryOptions options)
        {
            var query = queryable;

            if (!options.IncludeDraftPosts)
            {
                query = query.Where(x => !x.IsDraft);
            }
            if (!options.IncludeFuturePosts)
            {
                query = query.Where(x => x.PublishedAt < DateTimeOffset.UtcNow);
            }

            return query;
        }

        public static BlogPost? FirstOrDefaultById(
            this IQueryable<BlogPost> queryable,
            string id)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            return queryable.Where(x => x.Id == id).FirstOrDefault();
        }

        public static BlogPost? FirstOrDefaultBySlug(
            this IQueryable<BlogPost> queryable,
            string slug)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            return queryable.Where(x => x.Slug == slug).FirstOrDefault();
        }

        public static IOrderedQueryable<BlogPost> OrderByPublishedAt(
            this IQueryable<BlogPost> queryable)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.OrderBy(x => x.PublishedAt);
        }

        public static IOrderedQueryable<BlogPost> OrderByPublishedAtDescending(
            this IQueryable<BlogPost> queryable)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return queryable.OrderByDescending(x => x.PublishedAt);
        }

        public static List<BlogDateItem> ToDateList(
            this IQueryable<BlogPost> queryable,
            IEnumerable<BlogKey> blogKeys)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts = queryable.WhereBlogKeys(blogKeys);

            return
                (from post in posts
                 let publishedAt = post.PublishedAt
                 where publishedAt != null
                 let postMonth =
                    new DateTime(publishedAt.Value.Year, publishedAt.Value.Month, 1)
                 group post by postMonth into g
                 select new BlogDateItem
                 {
                     Count = g.Count(),
                     Date = g.Key
                 })
                 .OrderByDescending(x => x.Date)
                 .ToList();
        }

        public static List<BlogPostSeriesEpisode> ToSeriesList(
            this IQueryable<BlogPost> queryable,
            string seriesName,
            IEnumerable<BlogKey> blogKeys)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (seriesName is null)
                throw new ArgumentNullException(nameof(seriesName));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts = queryable.WhereBlogKeys(blogKeys);

            return
                posts
                    .Where(x =>
                        x.Series != null
                        && x.Series.EqualsIgnoreCase(seriesName))
                    .OrderBy(x => x.PublishedAt)
                    .Select((x, i) =>
                        new BlogPostSeriesEpisode(x, i + 1))
                    .ToList();
        }

        public static List<BlogTagItem> ToTagList(
            this IQueryable<BlogPost> queryable,
            IEnumerable<BlogKey> blogKeys)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts = queryable.WhereBlogKeys(blogKeys);

            return
                (from post in posts
                 from tag in post.Tags
                 group post by tag into g
                 select new BlogTagItem
                 {
                     Count = g.Count(),
                     Name = g.Key
                 })
                 .OrderBy(x => x.Name)
                 .ToList();
        }

        public static IQueryable<BlogPost> WhereBlogKey(
            this IQueryable<BlogPost> queryable,
            BlogKey blogKey)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable.Where(x => x.BlogKey == blogKey || !blogKey.HasValue);
        }

        public static IQueryable<BlogPost> WhereBlogKeys(
            this IQueryable<BlogPost> queryable,
            IEnumerable<BlogKey> blogKeys)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return
                queryable
                    .Where(x => !blogKeys.Any()
                        || blogKeys.Contains(x.BlogKey));
        }

        public static IQueryable<BlogPost> WhereContentContains(
            this IQueryable<BlogPost> queryable,
            string searchText)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));

            return
                queryable
                    .Where(x =>
                        x.Content.IndexOf(
                            searchText,
                            StringComparison.InvariantCultureIgnoreCase) >= 0
                        || x.Summary.IndexOf(
                            searchText,
                            StringComparison.InvariantCultureIgnoreCase) >= 0
                        || x.Title.IndexOf(
                            searchText,
                            StringComparison.InvariantCultureIgnoreCase) >= 0
                        || x.Tags.Any(t =>
                            t.IndexOf(
                                searchText,
                                StringComparison.InvariantCultureIgnoreCase) >= 0));
        }

        public static IQueryable<BlogPost> WhereTagName(
            this IQueryable<BlogPost> queryable,
            string tagName)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (tagName is null)
                throw new ArgumentNullException(nameof(tagName));

            return
                queryable
                    .Where(x =>
                        x.Tags.Any(t =>
                            t.Equals(
                                tagName,
                                StringComparison.InvariantCultureIgnoreCase)));
        }

        public static IQueryable<BlogPost> WherePublishedAfter(
            this IQueryable<BlogPost> queryable,
            DateTimeOffset? publishedAfter)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable
                    .Where(x =>
                        publishedAfter == null
                        || x.PublishedAt > publishedAfter);
        }

        public static IQueryable<BlogPost> WherePublishedAt(
            this IQueryable<BlogPost> queryable,
            DateTimeOffset publishedAt)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable
                    .Where(x =>
                        x.PublishedAt != null
                        && x.PublishedAt.Value.Year == publishedAt.Year
                        && x.PublishedAt.Value.Month == publishedAt.Month);
        }

        public static IQueryable<BlogPost> WhereUpdatedAfter(
            this IQueryable<BlogPost> queryable,
            DateTimeOffset? updatedAfter)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable
                    .Where(x => updatedAfter == null || x.UpdatedAt > updatedAfter);
        }
    }
}

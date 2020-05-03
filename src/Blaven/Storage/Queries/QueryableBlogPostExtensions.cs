﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Storage.Queries
{
    public static class QueryableBlogPostExtensions
    {
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

        public static List<BlogDateItem> ToDateList(
            this IQueryable<BlogPost> queryable,
            IEnumerable<BlogKey> blogKeys)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                queryable
                    .WhereBlogKeys(blogKeys);

            return
                (from post in posts
                 let publishedAt = post.PublishedAt
                 where publishedAt != null
                 let postMonth =
                    new DateTime(publishedAt.Value.Year, publishedAt.Value.Month, 1)
                 group post by new { post.BlogKey, Date = postMonth } into g
                 orderby g.Key.Date descending
                 select new BlogDateItem
                 {
                     BlogKey = g.Key.BlogKey,
                     Count = g.Count(),
                     Date = g.Key.Date
                 }).ToList();
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
                 group post by new { post.BlogKey, Tag = tag } into g
                 orderby g.Key.Tag ascending
                 select new BlogTagItem
                 {
                     BlogKey = g.Key.BlogKey,
                     Count = g.Count(),
                     Name = g.Key.Tag
                 }).ToList();
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
                            StringComparison.OrdinalIgnoreCase) >= 0);
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
                        x.Tags.Contains(
                            tagName, StringComparer.OrdinalIgnoreCase));
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
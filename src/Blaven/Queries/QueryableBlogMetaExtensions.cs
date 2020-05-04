using System;
using System.Linq;

namespace Blaven.Queries
{
    public static class QueryableBlogMetaExtensions
    {
        public static BlogMeta? FirstOrDefaultById(
            this IQueryable<BlogMeta> queryable,
            string id)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            return
                queryable
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
        }

        public static IQueryable<BlogMeta> WhereBlogKey(
            this IQueryable<BlogMeta> queryable,
            BlogKey blogKey)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable.Where(x => !blogKey.HasValue || x.BlogKey == blogKey);
        }

        public static IQueryable<BlogMeta> WhereUpdatedAfter(
            this IQueryable<BlogMeta> queryable,
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

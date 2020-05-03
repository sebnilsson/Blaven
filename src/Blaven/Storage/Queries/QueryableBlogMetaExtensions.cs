using System;
using System.Linq;

namespace Blaven.Storage.Queries
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

        public static IQueryable<BlogMeta> WhereUpdatedAt(
            this IQueryable<BlogMeta> queryable,
            DateTimeOffset? updatedAt)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return
                queryable
                    .Where(x => updatedAt == null || x.UpdatedAt > updatedAt);
        }
    }
}

using System;
using System.Linq;

namespace Blaven
{
    public static class QueryableExtensions
    {
        public static IPagedReadOnlyList<T> ToPagedList<T>(
            this IQueryable<T> queryable,
            Paging paging)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return new PagedReadOnlyList<T>(queryable, paging);
        }
    }
}

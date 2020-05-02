using System;
using System.Linq;

namespace Blaven
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPaging<T>(
            this IQueryable<T> queryable,
            Paging paging)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            return paging.Apply(queryable);
        }
    }
}

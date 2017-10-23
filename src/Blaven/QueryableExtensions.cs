using System;
using System.Linq;

namespace Blaven
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Paged<TSource>(
            this IQueryable<TSource> source,
            int pageSize,
            int pageIndex = 0)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var paged = PagingUtility.GetPaged(source, pageSize, pageIndex);
            return paged;
        }
    }
}
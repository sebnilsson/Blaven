using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Raven.Client;

namespace Blaven.DataStorage.RavenDb
{
    public static class QueryableExtensions
    {
        private const int PageSize = 1024;

        public static IReadOnlyList<T> ToListAll<T>(this IQueryable<T> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var result = new List<T>();

            IList<T> items;
            int i = 0;

            do
            {
                int skipCount = PagingUtility.GetSkip(PageSize, i);

                items = queryable.Skip(skipCount).Take(PageSize).ToList();

                result.AddRange(items);

                i++;
            }
            while (items.Count == PageSize);

            return result;
        }

        public static async Task<IReadOnlyList<T>> ToListAllAsync<T>(this IQueryable<T> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var result = new List<T>();

            IList<T> items;
            int i = 0;

            do
            {
                int skipCount = PagingUtility.GetSkip(PageSize, i);

                items = await queryable.Skip(skipCount).Take(PageSize).ToListAsync();

                result.AddRange(items);

                i++;
            }
            while (items.Count == PageSize);

            return result;
        }
    }
}
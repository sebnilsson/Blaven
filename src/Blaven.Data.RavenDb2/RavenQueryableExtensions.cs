using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.Data.RavenDb2
{
    public static class RavenQueryableExtensions
    {
        private const int PageSize = 1024;

        public static List<T> ToListAll<T>(this IRavenQueryable<T> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var listAll = ToListAllInternal(queryable).ToList();
            return listAll;
        }

        private static IEnumerable<T> ToListAllInternal<T>(IRavenQueryable<T> queryable)
        {
            List<T> items;
            int i = 0;

            do
            {
                int skipCount = PagingUtility.GetSkip(PageSize, i);

                items = queryable.Skip(skipCount).Take(PageSize).ToList();
                foreach (var item in items)
                {
                    yield return item;
                }

                i++;
            }
            while (items.Count == PageSize);
        }
    }
}
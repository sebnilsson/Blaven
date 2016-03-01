using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Blaven
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Distinct<TKey, TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return from item in source.GroupBy(keySelector) select item.First();
        }

        public static IEnumerable<TSource> Duplicates<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return from grouping in source.GroupBy(x => x) where grouping.Count() > 1 from item in grouping select item;
        }

        public static IEnumerable<TSource> Duplicates<TKey, TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            return from grouping in source.GroupBy(keySelector)
                   where grouping.Count() > 1
                   from item in grouping
                   select item;
        }

        public static IEnumerable<TSource> Paged<TSource>(this IEnumerable<TSource> source, int pageSize, int pageIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var paged = PagingUtility.GetPaged(source, pageSize: pageSize, pageIndex: pageIndex);
            return paged;
        }

        public static IEnumerable<TSource> Paged<TSource>(this IEnumerable<TSource> source, int pageSize)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var paged = source.Paged(pageSize: pageSize, pageIndex: 0);
            return paged;
        }

        public static IReadOnlyList<TSource> ToReadOnlyList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var list = source.ToList();
            return new ReadOnlyCollection<TSource>(list);
        }
    }
}
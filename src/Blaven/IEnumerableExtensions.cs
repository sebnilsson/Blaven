using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> Distinct<TSource>(
            this IEnumerable<TSource> source, Func<TSource, object> keySelector)
        {
            return from item in source.GroupBy(keySelector) select item.First();
        }

        public static IEnumerable<TSource> Duplicates<TSource>(this IEnumerable<TSource> source)
        {
            return from grouping in source.GroupBy(x => x) where grouping.Count() > 1 from item in grouping select item;
        }

        public static IEnumerable<TSource> Duplicates<TSource>(
            this IEnumerable<TSource> source, Func<TSource, object> keySelector)
        {
            return from grouping in source.GroupBy(keySelector)
                   where grouping.Count() > 1
                   from item in grouping
                   select item;
        }
    }
}
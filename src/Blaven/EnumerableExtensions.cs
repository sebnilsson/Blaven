using System;
using System.Collections.Generic;

namespace Blaven
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ApplyPaging<T>(
            this IEnumerable<T> enumerable,
            Paging paging)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            return paging.Apply(enumerable);
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            return new HashSet<T>(enumerable);
        }
    }
}

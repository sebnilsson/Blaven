using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public readonly struct Paging
    {
        public const int DefaultPageSize = 10;

        private readonly int _pageSize;

        public Paging(int index = 0, int size = DefaultPageSize)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (size < 0)
                throw new ArgumentOutOfRangeException(nameof(size));

            Index = index;
            _pageSize = size;
        }

        public readonly int Index { get; }

        public readonly int Size =>
            _pageSize > 0 ? _pageSize : DefaultPageSize;

        public IEnumerable<T> Apply<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var skipCount = Index * Size;
            var takeCount = Size;

            return
                enumerable
                    .Skip(skipCount)
                    .Take(takeCount);
        }

        public IQueryable<T> Apply<T>(IQueryable<T> queryable)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            var skipCount = Index * Size;
            var takeCount = Size;

            return
                queryable
                    .Skip(skipCount)
                    .Take(takeCount);
        }

        public static Paging Max => new Paging(index: 0, size: int.MaxValue);
    }
}

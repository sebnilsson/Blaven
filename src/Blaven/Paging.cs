using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public readonly struct Paging
    {
        public const int DefaultPageSize = 10;

        private readonly int _pageSize;

        public Paging(int pageIndex = 0, int pageSize = DefaultPageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize));

            PageIndex = pageIndex;
            _pageSize = pageSize;
        }

        public readonly int PageIndex { get; }

        public readonly int PageSize =>
            _pageSize > 0 ? _pageSize : DefaultPageSize;

        public IEnumerable<T> Apply<T>(IEnumerable<T> enumerable)
        {
            if (enumerable is null)
                throw new ArgumentNullException(nameof(enumerable));

            var skipCount = PageIndex * PageSize;
            var takeCount = PageSize;

            return
                enumerable
                    .Skip(skipCount)
                    .Take(takeCount);
        }

        public IQueryable<T> Apply<T>(IQueryable<T> queryable)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            var skipCount = PageIndex * PageSize;
            var takeCount = PageSize;

            return
                queryable
                    .Skip(skipCount)
                    .Take(takeCount);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public readonly struct Paging
    {
        public const int DefaultPageSize = 10;

        public Paging(int pageIndex = 0, int pageSize = DefaultPageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int PageIndex { get; }

        public int PageSize { get; }

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

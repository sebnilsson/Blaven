using System;
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

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public static class PagingUtility
    {
        public static IEnumerable<TSource> GetPaged<TSource>(IEnumerable<TSource> source, int pageSize, int pageIndex)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            var skip = GetSkip(pageSize, pageIndex);

            var paged = source;
            if (skip > 0)
                paged = paged.Skip(skip);

            paged = paged.Take(pageSize);

            return paged;
        }

        public static IQueryable<TSource> GetPaged<TSource>(IQueryable<TSource> source, int pageSize, int pageIndex)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            var skip = GetSkip(pageSize, pageIndex);

            var paged = source;
            if (skip > 0)
                paged = paged.Skip(skip);

            paged = paged.Take(pageSize);

            return paged;
        }

        public static int GetSkip(int pageSize, int pageIndex)
        {
            if (pageSize < 1)
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(pageIndex));

            var skip = pageSize * pageIndex;
            return skip;
        }
    }
}
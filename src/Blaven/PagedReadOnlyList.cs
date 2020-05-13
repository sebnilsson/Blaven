using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class PagedReadOnlyList<T> : IPagedReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> _list;

        public PagedReadOnlyList(IQueryable<T> queryable, Paging paging)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));

            _list = paging.Apply(queryable).ToList();

            PageIndex = paging.Index;
            PageSize = paging.Size;

            HasPrevious = paging.Index > 0;

            HasNext = GetHasNext(queryable, paging);
        }

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public bool HasNext { get; }

        public bool HasPrevious { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        private static bool GetHasNext(IQueryable<T> queryable, Paging paging)
        {
            var nextItemPaging = new Paging(index: paging.Index + 1, size: 1);

            var nextItemList = nextItemPaging.Apply(queryable);

            return nextItemList.Any();
        }
    }
}

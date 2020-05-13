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

            HasPrevious = paging.Index > 0;

            var nextItemPaging = new Paging(index: paging.Index + 1, size: 1);

            var nextItemList = nextItemPaging.Apply(queryable);

            HasNext = nextItemList.Any();
        }

        public bool HasNext { get; }

        public bool HasPrevious { get; }

        public T this[int index] => _list[index];

        public int Count => _list.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}

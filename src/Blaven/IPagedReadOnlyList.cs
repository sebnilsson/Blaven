using System.Collections.Generic;

namespace Blaven
{
    public interface IPagedReadOnlyList<T> : IReadOnlyList<T>
    {
        bool HasNext { get; }

        bool HasPrevious { get; }
    }
}

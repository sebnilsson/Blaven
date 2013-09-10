using System.Collections.Generic;
using System.Linq;

namespace Blaven.RavenDb
{
    public static class IEnumerableFilterExtensions
    {
        public static IList<T> ToListHandled<T>(this IEnumerable<T> query)
        {
            return RavenDbHelper.HandleRavenExceptions(() => query.ToList());
        }
    }
}
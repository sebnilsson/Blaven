using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Blaven
{
    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(
            this NameValueCollection nameValueCollection,
            IEqualityComparer<string> comparer = null)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            var dictionary = nameValueCollection.AllKeys.ToDictionary(x => x, x => nameValueCollection[x], comparer);
            return dictionary;
        }

        public static Dictionary<string, string> ToDictionaryIgnoreCase(this NameValueCollection nameValueCollection)
        {
            if (nameValueCollection == null)
            {
                throw new ArgumentNullException(nameof(nameValueCollection));
            }

            var dictionary = nameValueCollection.ToDictionary(StringComparer.InvariantCultureIgnoreCase);
            return dictionary;
        }
    }
}
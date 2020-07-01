using System;
using System.Linq;

namespace Blaven
{
    public static class StringExtensions
    {
        public static string Coalesce(this string str, params string[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            return
                new[] { str }
                .Concat(values)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        }

        public static bool ContainsIgnoreCase(this string str, string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (str == null)
            {
                return false;
            }

            var indexOf =
                str.IndexOf(
                    value,
                    StringComparison.InvariantCultureIgnoreCase);

            return indexOf >= 0;
        }

        public static bool EqualsIgnoreCase(this string str, string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            if (str == null)
            {
                return false;
            }

            return
                str.Equals(
                    value,
                    StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

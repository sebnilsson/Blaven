using System;

namespace Blaven
{
    public static class StringExtensions
    {
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
                    StringComparison.CurrentCultureIgnoreCase);

            return indexOf >= 0;
        }
    }
}

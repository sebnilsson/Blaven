using System;

namespace Blaven
{
    internal static class Parameters
    {
        public static void ThrowIfArgumentNull<T>(T obj, string paramName, string message = null)
        {
            if (obj != null)
            {
                return;
            }

            if (message != null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            throw new ArgumentNullException(paramName);
        }

        public static void ThrowIfArgumentOutOfRange<T>(
            T obj,
            Func<T, bool> predicate,
            string paramName,
            string message = null)
        {
            bool isOutOfRange = predicate(obj);
            if (!isOutOfRange)
            {
                return;
            }

            if (message != null)
            {
                throw new ArgumentNullException(paramName, message);
            }

            throw new ArgumentNullException(paramName);
        }
    }
}
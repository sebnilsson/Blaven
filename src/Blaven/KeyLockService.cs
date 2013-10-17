using System;
using System.Collections.Concurrent;

namespace Blaven
{
    internal static class KeyLockService
    {
        private static readonly ConcurrentDictionary<string, object> KeyLocks =
            new ConcurrentDictionary<string, object>();

        //public static void PerformLockedAction(string key, Action action)
        //{
        //    var keyLock = GetKeyLock(key);
        //    lock (keyLock)
        //    {
        //        action();
        //    }
        //}

        public static TResult PerformLockedFunction<TResult>(string key, Func<TResult> func)
        {
            var keyLock = GetKeyLock(key);
            lock (keyLock)
            {
                return func();
            }
        }

        private static object GetKeyLock(string key)
        {
            return KeyLocks.GetOrAdd(key, new object());
        }
    }
}
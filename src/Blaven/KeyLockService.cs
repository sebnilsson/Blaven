using System;
using System.Collections.Generic;

namespace Blaven
{
    public static class KeyLockService
    {
        private static readonly object KeyLock = new object();

        private static readonly Dictionary<string, object> KeyLocks = new Dictionary<string, object>();

        public static void PerformLockedAction(string key, Action action)
        {
            var keyLock = GetKeyLock(key);
            lock (keyLock)
            {
                action();
            }
        }

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
            lock (KeyLock)
            {
                if (!KeyLocks.ContainsKey(key))
                {
                    KeyLocks[key] = new object();
                }

                return KeyLocks[key];
            }
        }
    }
}
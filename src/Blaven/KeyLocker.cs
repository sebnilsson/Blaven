﻿using System;
using System.Collections.Concurrent;

namespace Blaven
{
    public class KeyLocker : KeyLocker<string>
    {
    }

    public class KeyLocker<TKey>
    {
        private readonly ConcurrentDictionary<TKey, object> _locks = new ConcurrentDictionary<TKey, object>();

        public object GetLock(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return _locks.GetOrAdd(key, s => new object());
        }

        public void RemoveLock(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            object o;
            _locks.TryRemove(key, out o);
        }

        public TResult RunWithLock<TResult>(TKey key, Func<TResult> body)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            lock (_locks.GetOrAdd(key, s => new object()))
            {
                return body();
            }
        }

        public void RunWithLock(TKey key, Action body)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            lock (_locks.GetOrAdd(key, s => new object()))
            {
                body();
            }
        }
    }
}
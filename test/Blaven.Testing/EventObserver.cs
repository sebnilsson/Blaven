using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class EventObserver : EventObserver<string>
    {
        public EventObserver(Action<object, string> callback = null)
            : base(callback)
        {
        }
    }

    public class EventObserver<TEventArgs>
    {
        private readonly object isRunningsLock = new object();

        private readonly object runCountLock = new object();

        private readonly HashSet<TEventArgs> isRunnings =
            new HashSet<TEventArgs>();

        private readonly ConcurrentDictionary<TEventArgs, int> runningCollisions =
            new ConcurrentDictionary<TEventArgs, int>();

        public EventObserver(Action<object, TEventArgs> callback = null)
        {
            this.Handler = (sender, key) =>
                {
                    lock (this.runCountLock)
                    {
                        this.RunCount = this.RunCount + 1;
                    }

                    bool isRunning = false;

                    lock (this.isRunningsLock)
                    {
                        isRunning = this.isRunnings.Contains(key);
                    }


                    if (isRunning)
                    {
                        this.runningCollisions.AddOrUpdate(key, _ => 1, (_, oldValue) => (oldValue + 1));
                    }

                    lock (this.isRunningsLock)
                    {
                        this.isRunnings.Add(key);
                    }

                    callback?.Invoke(sender, key);

                    lock (this.isRunningsLock)
                    {
                        if (this.isRunnings.Contains(key))
                        {
                            this.isRunnings.Remove(key);
                        }
                    }
                };
        }

        public int CollisionCount
        {
            get
            {
                return this.runningCollisions.Keys.Sum(x => this.runningCollisions[x]);
            }
        }

        public EventHandler<TEventArgs> Handler { get; }

        public int RunCount { get; private set; }
    }
}
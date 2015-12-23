using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Blaven.Tests
{
    [DebuggerDisplay("TKey={typeof(TKey)}, Events={Events.Count}")]
    public class DelegateTracker<TKey>
    {
        private readonly IList<DelegateTrackerEvent<TKey>> events;

        public DelegateTracker()
            : this(null)
        {
        }

        internal DelegateTracker(IEnumerable<DelegateTrackerEvent<TKey>> events)
        {
            events = (events ?? Enumerable.Empty<DelegateTrackerEvent<TKey>>()).Where(x => x != null);

            this.events = new List<DelegateTrackerEvent<TKey>>(events);
        }

        public IReadOnlyCollection<DelegateTrackerEvent<TKey>> Events
            => new ReadOnlyCollection<DelegateTrackerEvent<TKey>>(this.events);

        public IReadOnlyDictionary<TKey, int> KeyCollisionCount => this.GetKeyCollisionCount();

        public IReadOnlyDictionary<TKey, int> KeyRunCount => this.GetKeyRunCount();

        public IReadOnlyDictionary<TKey, int> KeyRunOtherCount => this.GetKeyRunOtherCount();

        public int RunCount => this.Events.Count;

        public DelegateTrackerEvent<TKey> AddEvent(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            int threadId = Thread.CurrentThread.ManagedThreadId;

            var @event = new DelegateTrackerEvent<TKey>(key, threadId);

            lock (this.events)
            {
                this.events.Add(@event);
            }

            return @event;
        }

        private IReadOnlyDictionary<TKey, int> GetKeyCollisionCount()
        {
            var collisionCount =
                this.Events.Where(
                    x =>
                    this.Events.Any(
                        y => x != y && Equals(x.Key, y.Key) && (y.StartedAt >= x.StartedAt && y.EndedAt <= x.EndedAt)))
                    .GroupBy(x => x.Key, x => x)
                    .ToDictionary(x => x.Key, x => x.Count());
            return collisionCount;
        }

        private IReadOnlyDictionary<TKey, int> GetKeyRunCount()
        {
            var runCount =
                this.Events.GroupBy(x => x.Key, x => x)
                    .Select(x => new { x.Key, Count = x.Count() })
                    .ToDictionary(x => x.Key, x => x.Count);
            return runCount;
        }

        private IReadOnlyDictionary<TKey, int> GetKeyRunOtherCount()
        {
            var keys = this.Events.GroupBy(x => x.Key, x => x).Select(x => x.Key).ToList();

            var runOtherCount = (from key in keys
                                 let keyEvents = this.Events.Where(x => Equals(x.Key, key))
                                 let otherEvents = this.Events.Where(x => !Equals(x.Key, key))
                                 let runOthers =
                                     otherEvents.Where(
                                         x => keyEvents.Any(y => x.StartedAt >= y.StartedAt && x.EndedAt <= y.EndedAt))
                                 select new { Key = key, Count = runOthers.Count() }).ToDictionary(
                                     x => x.Key,
                                     x => x.Count);
            return runOtherCount;
        }
    }
}
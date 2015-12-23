using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("TKey={typeof(TKey)}, Key={Key}, ThreadId={ThreadId}, StartedAt={StartedAt}, EndedAt={EndedAt}")]
    public class DelegateTrackerEvent<TKey>
    {
        public DelegateTrackerEvent(TKey key, int threadId)
            : this(key, threadId, DateTime.Now, endedAt: null)
        {
        }

        internal DelegateTrackerEvent(TKey key, int threadId, DateTime startedAt, DateTime? endedAt)
        {
            this.Key = key;
            this.ThreadId = threadId;
            this.StartedAt = startedAt;
            this.EndedAt = endedAt;
        }

        public DateTime? EndedAt { get; internal set; }

        public TKey Key { get; }

        public int ThreadId { get; set; }

        public DateTime StartedAt { get; }
    }
}
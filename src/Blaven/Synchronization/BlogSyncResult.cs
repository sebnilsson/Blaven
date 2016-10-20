using System;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay("BlogKey={BlogKey}, StartedAt={StartedAt}, Elapsed={Elapsed}, ElapsedMs={ElapsedMs}")]
    public class BlogSyncResult : BlogKeyItemBase
    {
        private readonly Stopwatch stopwatch;

        public BlogSyncResult(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            this.BlogKey = blogKey;

            this.StartedAt = DateTime.Now;

            this.stopwatch = Stopwatch.StartNew();
        }

        public BlogMeta BlogMeta { get; private set; }

        public BlogSourceChangeSet ChangeSet { get; private set; }

        public TimeSpan Elapsed { get; private set; }

        public double ElapsedMs => this.Elapsed.TotalMilliseconds;

        public DateTime StartedAt { get; }

        public void OnDataUpdated(BlogMeta blogMeta, BlogSourceChangeSet changeSet)
        {
            this.BlogMeta = blogMeta;
            this.ChangeSet = changeSet;
        }

        public void OnDone()
        {
            this.stopwatch.Stop();

            this.Elapsed = this.stopwatch.Elapsed;
        }
    }
}
using System;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay(
        "BlogKey={BlogKey}, IsUpdated={IsUpdated}, StartedAt={StartedAt}, Elapsed={Elapsed}, ElapsedMs={ElapsedMs}")]
    public class BlogSyncResult
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

        public string BlogKey { get; }

        public bool IsUpdated { get; private set; }

        public TimeSpan Elapsed { get; private set; }

        public double ElapsedMs => this.Elapsed.TotalMilliseconds;

        public DateTime StartedAt { get; }

        public void HandleDone(bool isUpdated)
        {
            this.IsUpdated = isUpdated;

            this.stopwatch.Stop();

            this.Elapsed = this.stopwatch.Elapsed;
        }
    }
}
using System;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay("BlogKey={BlogKey}, Elapsed={Elapsed}, ElapsedMs={ElapsedMs}")]
    public class BlogSyncResult : BlogKeyItemBase
    {
        public BlogSyncResult(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            this.BlogKey = blogKey;
        }

        public BlogMeta BlogMeta { get; private set; }

        public BlogSyncChangeSet ChangeSet { get; private set; }

        public TimeSpan Elapsed { get; internal set; }

        public double ElapsedMs => this.Elapsed.TotalMilliseconds;

        internal void OnDataUpdated(BlogMeta blogMeta, BlogSyncChangeSet changeSet)
        {
            this.BlogMeta = blogMeta;
            this.ChangeSet = changeSet;
        }
    }
}
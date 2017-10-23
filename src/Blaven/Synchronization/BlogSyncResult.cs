using System;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay("BlogKey={BlogKey}, Elapsed={Elapsed}, ElapsedMs={ElapsedMs}")]
    public class BlogSyncResult : BlogKeyItemBase
    {
        internal BlogSyncResult(string blogKey)
        {
            BlogKey = blogKey ?? throw new ArgumentNullException(nameof(blogKey));
        }

        public BlogMeta BlogMeta { get; private set; }

        public BlogSyncPostsChangeSet BlogPostsChanges { get; private set; }

        public TimeSpan Elapsed { get; internal set; }

        public double ElapsedMs => Elapsed.TotalMilliseconds;

        internal void OnDataUpdated(BlogMeta blogMeta, BlogSyncPostsChangeSet changeSet)
        {
            BlogMeta = blogMeta;
            BlogPostsChanges = changeSet;
        }
    }
}
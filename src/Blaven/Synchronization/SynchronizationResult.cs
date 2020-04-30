using System;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay("BlogKey={BlogKey}, Elapsed={Elapsed}, ElapsedMs={ElapsedMs}")]
    public class SynchronizationResult
    {
        public SynchronizationResult(
            BlogKey blogKey,
            BlogMeta? meta,
            SynchronizationBlogPosts posts,
            TimeSpan elapsed)
        {
            BlogKey = blogKey;
            Meta = meta;
            Posts = posts;
            Elapsed = elapsed;
        }

        public BlogKey BlogKey { get; }

        public TimeSpan Elapsed { get; }

        public double ElapsedMs => Elapsed.TotalMilliseconds;

        public BlogMeta? Meta { get; }

        public SynchronizationBlogPosts Posts { get; }
    }
}

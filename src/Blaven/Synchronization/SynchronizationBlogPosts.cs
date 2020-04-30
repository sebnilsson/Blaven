using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blaven.Synchronization
{
    [DebuggerDisplay("Inserted={Inserted.Count}, Updated={Updated.Count}, Deleted={Deleted.Count}")]
    public class SynchronizationBlogPosts
    {
        public SynchronizationBlogPosts(
            IEnumerable<BlogPost> inserted,
            IEnumerable<BlogPost> updated,
            IEnumerable<BlogPostBase> deleted)
        {
            if (inserted is null)
                throw new ArgumentNullException(nameof(inserted));
            if (updated is null)
                throw new ArgumentNullException(nameof(updated));
            if (deleted is null)
                throw new ArgumentNullException(nameof(deleted));

            Inserted = inserted.ToList();
            Updated = updated.ToList();
            Deleted = deleted.ToList();
        }

        public IReadOnlyList<BlogPostBase> Deleted { get; }

        public IReadOnlyList<BlogPost> Inserted { get; }

        public IReadOnlyList<BlogPost> Updated { get; }
    }
}

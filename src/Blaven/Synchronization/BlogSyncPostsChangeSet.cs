using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven.Synchronization
{
    [DebuggerDisplay(
         "BlogKey={BlogKey}, Inserted={InsertedBlogPosts.Count}, Updated={UpdatedBlogPosts.Count}, Deleted={DeletedBlogPosts.Count}"
     )]
    public class BlogSyncPostsChangeSet : BlogKeyItemBase
    {
        internal BlogSyncPostsChangeSet(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            this.BlogKey = blogKey;
        }

        // TODO: Move BlogMeta into here?

        public List<BlogPostBase> DeletedBlogPosts { get; } = new List<BlogPostBase>();

        public List<BlogPost> InsertedBlogPosts { get; } = new List<BlogPost>();

        public List<BlogPost> UpdatedBlogPosts { get; } = new List<BlogPost>();
    }
}
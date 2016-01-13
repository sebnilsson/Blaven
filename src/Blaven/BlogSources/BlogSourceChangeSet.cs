using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven.BlogSources
{
    [DebuggerDisplay(
        "Inserted={InsertedBlogPosts.Count}, Updated={UpdatedBlogPosts.Count}, Deleted={DeletedBlogPosts.Count}")]
    public class BlogSourceChangeSet
    {
        public List<BlogPostBase> DeletedBlogPosts { get; } = new List<BlogPostBase>();

        public List<BlogPost> InsertedBlogPosts { get; } = new List<BlogPost>();

        public List<BlogPost> UpdatedBlogPosts { get; } = new List<BlogPost>();
    }
}
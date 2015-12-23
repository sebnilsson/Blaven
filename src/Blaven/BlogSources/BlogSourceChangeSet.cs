using System.Collections.Generic;

namespace Blaven.BlogSources
{
    public class BlogSourceChangeSet
    {
        public List<BlogPostBase> DeletedBlogPosts { get; } = new List<BlogPostBase>();

        public List<BlogPost> InsertedBlogPosts { get;  } = new List<BlogPost>();

        public List<BlogPost> UpdatedBlogPosts { get; } = new List<BlogPost>();
    }
}
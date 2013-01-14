using System.Collections.Generic;
using System.Linq;

namespace Blaven.Test
{
    public static class BlogPostsTestHelper
    {
        public static IEnumerable<BlogPost> GetBlogPosts(string blogKey, int count)
        {
            return GetBlogPosts(blogKey, 1, count);
        }

        public static IEnumerable<BlogPost> GetBlogPosts(string blogKey, int start, int count)
        {
            var numbers = Enumerable.Range(start, count);
            return numbers.Select(number => new BlogPost(blogKey, (uint)number));
        }
    }
}
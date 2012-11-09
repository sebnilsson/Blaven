using System.Collections.Generic;
using System.Linq;

namespace Blaven.Test
{
    internal static class BlogDataTestHelper
    {
        public static BlogData GetBlogData(string blogKey, int postsCount)
        {
            var posts = BlogPostsTestHelper.GetBlogPosts(blogKey, postsCount);
            return GetBlogData(blogKey, posts);
        }

        public static BlogData GetBlogData(string blogKey, IEnumerable<BlogPost> posts = null)
        {
            posts = posts ?? Enumerable.Empty<BlogPost>();

            return new BlogData { Info = new BlogInfo { BlogKey = blogKey, Title = "TEST_TITLE", }, Posts = posts, };
        }
    }
}
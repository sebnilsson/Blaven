using System.Collections.Generic;
using System.Linq;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static IEnumerable<BlogPostBase> GetBlogPostBases(
            int start,
            int count,
            string blogKey = BlogKey,
            bool isUpdate = false)
        {
            var blogPostBases =
                Enumerable.Range(start, count).Select(i => GetBlogPostBase(blogKey, i, isUpdate)).ToList();
            return blogPostBases;
        }

        public static BlogPostBase GetBlogPostBase(string blogKey, int index = 0, bool isUpdate = false)
        {
            var blogPost = GetBlogPost(blogKey, index, isUpdate);
            return blogPost;
        }
    }
}
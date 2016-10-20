using System.Collections.Generic;
using System.Linq;

namespace Blaven.Tests
{
    public static class BlogPostBaseTestData
    {
        public static IEnumerable<BlogPostBase> CreateCollection(
            int start,
            int count,
            string blogKey = BlogMetaTestData.BlogKey,
            bool isUpdate = false)
        {
            var blogPostBases = Enumerable.Range(start, count).Select(i => Create(blogKey, i, isUpdate)).ToList();
            return blogPostBases;
        }

        public static BlogPostBase Create(string blogKey, int index = 0, bool isUpdate = false)
        {
            var blogPost = BlogPostTestData.Create(blogKey, index, isUpdate);
            return blogPost;
        }
    }
}
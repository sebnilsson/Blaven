using System.Collections.Generic;
using System.Linq;

namespace Blaven.Testing
{
    public static class BlogPostTheoryData
    {
        public static IEnumerable<object[]> GetDbBlogPostsForMultipleKeys(int start, int count)
        {
            var blogPosts1 = BlogPostTestData.CreateCollection(start, count, BlogMetaTestData.BlogKey);
            var blogPosts2 = BlogPostTestData.CreateCollection(start, count, BlogMetaTestData.BlogKey2);
            var blogPosts3 = BlogPostTestData.CreateCollection(start, count, BlogMetaTestData.BlogKey3);

            var blogPosts = blogPosts1.Concat(blogPosts2).Concat(blogPosts3).ToList();
            yield return new object[] { blogPosts };
        }

        public static IEnumerable<object[]> GetDbBlogPostsForSingleAndMultipleKeys(int start, int count)
        {
            foreach (var obj in GetDbBlogPostsForSingleKey(start, count))
                yield return obj;

            foreach (var obj in GetDbBlogPostsForMultipleKeys(start, count))
                yield return obj;
        }

        public static IEnumerable<object[]> GetDbBlogPostsForSingleKey(int start, int count)
        {
            yield return new object[] { BlogPostTestData.CreateCollection(start, count) };
        }
    }
}
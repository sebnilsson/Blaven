using System;

namespace Blaven
{
    public class SourceIdBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        public string GetBlavenId(BlogPostHead blogPost)
        {
            string blavenId = GetBlogPostBlavenId(blogPost);
            return blavenId;
        }

        public static string GetBlogPostBlavenId(BlogPostBase blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }
            if (string.IsNullOrWhiteSpace(blogPost.SourceId))
            {
                string message = $"{nameof(blogPost.SourceId)} cannot be null or empty.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }

            return blogPost.SourceId;
        }
    }
}
using System;

namespace Blaven
{
    public class SourceIdBlogPostBlavenIdProvider : IBlogPostBlavenIdProvider
    {
        public static string GetBlogPostBlavenId(BlogPostBase blogPost)
        {
            if (blogPost == null)
                throw new ArgumentNullException(nameof(blogPost));
            if (string.IsNullOrWhiteSpace(blogPost.SourceId))
            {
                var message = $"{nameof(blogPost.SourceId)} cannot be null or empty.";
                throw new ArgumentOutOfRangeException(nameof(blogPost), message);
            }

            return blogPost.SourceId;
        }

        public string GetBlavenId(BlogPostHead blogPost)
        {
            var blavenId = GetBlogPostBlavenId(blogPost);
            return blavenId;
        }
    }
}
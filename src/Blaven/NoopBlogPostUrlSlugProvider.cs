using System;

namespace Blaven
{
    public class NoopBlogPostUrlSlugProvider : IBlogPostUrlSlugProvider
    {
        public string GetSlug(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }

            return blogPost.UrlSlug;
        }
    }
}
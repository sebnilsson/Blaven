using System;

namespace Blaven
{
    public class NoopBlogPostUrlSlugProvider : IBlogPostUrlSlugProvider
    {
        public string GetUrlSlug(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }

            return blogPost.UrlSlug;
        }
    }
}
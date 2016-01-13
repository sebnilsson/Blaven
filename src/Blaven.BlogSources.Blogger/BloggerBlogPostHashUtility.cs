using System;

using Google.Apis.Blogger.v3.Data;

namespace Blaven.BlogSources.Blogger
{
    internal static class BloggerBlogPostHashUtility
    {
        public static string GetHash(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            string hash = post.Updated?.ToString("o") ?? post.ETag;
            return hash;
        }
    }
}
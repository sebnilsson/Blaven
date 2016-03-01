using System;
using System.Linq;

using Google.Apis.Blogger.v3.Data;

namespace Blaven.BlogSources.Blogger
{
    public static class BloggerDataConverter
    {
        public static BlogMeta ConvertMeta(Blog blog)
        {
            if (blog == null)
            {
                throw new ArgumentNullException(nameof(blog));
            }

            var blogMeta = new BlogMeta
                               {
                                   Description = blog.Description,
                                   Name = blog.Name,
                                   PublishedAt = blog.Published,
                                   SourceId = blog.Id,
                                   UpdatedAt = blog.Updated,
                                   Url = blog.Url
                               };
            return blogMeta;
        }

        public static BlogPost ConvertPost(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            var author = new BlogAuthor
                             {
                                 ImageUrl = post.Author?.Image?.Url,
                                 Name = post.Author?.DisplayName,
                                 SourceId = post.Author?.Id,
                                 Url = post.Author?.Url
                             };

            // Properties set by BlogSyncServiceUpdatePostsHelper: BlavenId, UrlSlug
            var blogPost = new BlogPost
                               {
                                   Author = author,
                                   Content = post.Content,
                                   Hash = GetBlogPostHash(post),
                                   PublishedAt = post.Published,
                                   SourceId = post.Id,
                                   SourceUrl = post.Url,
                                   Tags = post.Labels?.ToList() ?? Enumerable.Empty<string>(),
                                   Title = post.Title,
                                   UpdatedAt = post.Updated
                               };
            return blogPost;
        }

        private static string GetBlogPostHash(Post post)
        {
            if (post == null)
            {
                throw new ArgumentNullException(nameof(post));
            }

            string hash = post.Updated?.ToUniversalTime().ToString("o") ?? post.ETag;
            return hash;
        }
    }
}
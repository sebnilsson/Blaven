﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources.Blogger
{
    public static class BloggerDataConverter
    {
        public static BlogMeta ConvertMeta(BloggerBlogData blog)
        {
            if (blog == null)
                throw new ArgumentNullException(nameof(blog));

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

        public static BlogPost ConvertPost(BloggerPostData post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

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
                               BlogAuthor = author,
                               Content = post.Content,
                               Hash = GetBlogPostHash(post),
                               PublishedAt = post.Published,
                               SourceId = post.Id,
                               SourceUrl = post.Url,
                               BlogPostTags =
                                   post.Labels?.Select(x => new BlogPostTag(x)).ToList()
                                   ?? new List<BlogPostTag>(),
                               Title = post.Title,
                               UpdatedAt = post.Updated
                           };
            return blogPost;
        }

        private static string GetBlogPostHash(BloggerPostData post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            var hash = post.Updated?.ToUniversalTime().ToString("o") ?? post.ETag;
            return hash;
        }
    }
}
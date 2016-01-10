using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Blogger.v3.Data;

namespace Blaven.Sources.Blogger
{
    internal static class BloggerParser
    {
        public static BlogData ParseBlogData(BlavenBlogSetting settings, Blog bloggerBlog, PostList modifiedPosts)
        {
            try
            {
                return ParseBlogDataImpl(settings, bloggerBlog, modifiedPosts);
            }
            catch (Exception ex)
            {
                throw new BloggerParserException(settings.BlogKey, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(BlavenBlogSetting settings, Blog bloggerBlog, PostList modifiedPosts)
        {
            var blogInfo = new BlogInfo
                               {
                                   BlogKey = settings.BlogKey,
                                   Subtitle = bloggerBlog.Description,
                                   Title = bloggerBlog.Name,
                                   Updated = bloggerBlog.Updated,
                                   Url = bloggerBlog.Url
                               };

            var posts = ParseBlogPostsImpl(settings, modifiedPosts);

            return new BlogData { Info = blogInfo, Posts = posts.ToList() };
        }

        private static IEnumerable<BlogPost> ParseBlogPostsImpl(BlavenBlogSetting settings, PostList modifiedPosts)
        {
            var posts = from item in modifiedPosts.Items
                        let authorImageUrl =
                            (item.Author != null && item.Author.Image != null) ? item.Author.Image.Url : null
                        let authorName = (item.Author != null) ? item.Author.DisplayName : null
                        select
                            new BlogPost(settings.BlogKey, item.Id)
                                {
                                    Tags = item.Labels,
                                    Content = item.Content,
                                    SourceUrl = item.Url,
                                    Checksum = item.ETag,
                                    PublishedAt = item.Published ?? DateTime.MinValue,
                                    Title = item.Title,
                                    UpdatedAt = item.Updated,
                                    UrlSlug = BlavenBlogPostSlugProvider.Create(item.Title),
                                    Author =
                                        new BlogAuthor
                                            {
                                                ImageUrl = authorImageUrl,
                                                Name = authorName,
                                            }
                                };
            return posts;
        }
    }
}
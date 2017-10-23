using System;
using System.Collections.Generic;
using System.Linq;
using Blaven.Synchronization;

namespace Blaven.Testing
{
    public static class BlogPostTestData
    {
        public const int DefaultBlogPostsCount = 11;
        public const int DefaultTagCount = 5;

        public static string BlavenId1 => CreateBlavenId(1);

        public static string BlavenId2 => CreateBlavenId(2);

        public static string BlavenId3 => CreateBlavenId(3);

        public static DateTime TestPublishedAt => new DateTime(2015, 1, 1, 12, 30, 45);

        public static DateTime TestUpdatedAt => new DateTime(2015, 2, 2, 14, 45, 30);

        public static BlogPost Create(
            int index = 0,
            string blogKey = BlogMetaTestData.BlogKey,
            bool isUpdate = false,
            string hashPrefix = null,
            int updatedAtAddedDays = 0,
            DateTime? updatedAt = null,
            int tagCount = DefaultTagCount)
        {
            var author = new BlogAuthor
                         {
                             ImageUrl = TestUtility.GetTestString(
                                 nameof(BlogPost.BlogAuthor) + nameof(BlogAuthor.ImageUrl),
                                 blogKey,
                                 index,
                                 isUpdate),
                             Name = TestUtility.GetTestString(
                                 nameof(BlogPost.BlogAuthor) + nameof(BlogAuthor.Name),
                                 blogKey,
                                 index,
                                 isUpdate),
                             SourceId = TestUtility.GetTestString(
                                 nameof(BlogPost.BlogAuthor) + nameof(BlogAuthor.SourceId),
                                 blogKey,
                                 index,
                                 isUpdate),
                             Url = TestUtility.GetTestString(
                                 nameof(BlogPost.BlogAuthor) + nameof(BlogAuthor.Url),
                                 blogKey,
                                 index,
                                 isUpdate)
                         };

            var updatedAtValue = updatedAt ?? TestUpdatedAt.AddDays(updatedAtAddedDays + index);

            var blogPost = new BlogPost
                           {
                               BlogAuthor = author,
                               BlogKey = blogKey,
                               Content = TestUtility.GetTestString(
                                             nameof(BlogPost.Content),
                                             blogKey,
                                             index,
                                             isUpdate) + $"{Environment.NewLine}More content",
                               Hash = TestUtility.GetTestString(
                                   nameof(BlogPost.Hash),
                                   $"{hashPrefix}{blogKey}",
                                   index),
                               ImageUrl = TestUtility.GetTestString(
                                   nameof(BlogPost.ImageUrl),
                                   blogKey,
                                   index,
                                   isUpdate),
                               PublishedAt = TestPublishedAt.AddDays(index),
                               SourceId = CreatePostSourceId(index, blogKey),
                               SourceUrl = TestUtility.GetTestString(
                                   nameof(BlogPost.SourceUrl),
                                   blogKey,
                                   index,
                                   isUpdate),
                               Summary = TestUtility.GetTestString(
                                   nameof(BlogPost.Summary),
                                   blogKey,
                                   index,
                                   isUpdate),
                               BlogPostTags =
                                   CreatePostTags(index, tagCount)
                                       .Select(x => new BlogPostTag(x))
                                       .ToList(),
                               Title = TestUtility.GetTestString(
                                   nameof(BlogPost.Title),
                                   blogKey,
                                   index,
                                   isUpdate),
                               UpdatedAt = updatedAtValue
                           };

            blogPost.UrlSlug = BlogSyncConfigurationDefaults.SlugProvider.GetUrlSlug(blogPost);

            blogPost.BlavenId = BlogSyncConfigurationDefaults.BlavenIdProvider.GetBlavenId(blogPost);

            return blogPost;
        }

        public static string CreateBlavenId(int index, string blogKey = BlogMetaTestData.BlogKey)
        {
            var blogPost = Create(index, blogKey);

            return blogPost.BlavenId;
        }

        public static IReadOnlyList<BlogPost> CreateCollection(
            int start = 0,
            int count = DefaultBlogPostsCount,
            string blogKey = BlogMetaTestData.BlogKey,
            string hashPrefix = null)
        {
            var posts = Enumerable.Range(start, count)
                .Select(x => Create(blogKey: blogKey, index: x, hashPrefix: hashPrefix))
                .ToReadOnlyList();
            return posts;
        }

        public static string CreatePostSourceId(int index, string blogKey = BlogMetaTestData.BlogKey)
        {
            var postId = $"[{blogKey}]-Post Source Id-{index + 1}";
            return postId;
        }

        public static string CreatePostTag(int index)
        {
            var tag = $"Test Tag {index + 1}";
            return tag;
        }

        public static IEnumerable<string> CreatePostTags(int start, int count)
        {
            var tags = Enumerable.Range(start, count).Select(CreatePostTag);
            return tags;
        }
    }
}
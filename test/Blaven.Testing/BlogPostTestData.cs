using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Synchronization;

namespace Blaven.Tests
{
    public static class BlogPostTestData
    {
        public const int DefaultTagCount = 5;

        public static string BlavenId1 => CreateBlavenId(1);

        public static string BlavenId2 => CreateBlavenId(2);

        public static string BlavenId3 => CreateBlavenId(3);

        public static DateTime TestPublishedAt => new DateTime(2015, 1, 1, 12, 30, 45);

        public static DateTime TestUpdatedAt => new DateTime(2015, 2, 2, 14, 45, 30);

        public static string CreateBlavenId(int index, string blogKey = BlogMetaTestData.BlogKey)
        {
            var blogPost = Create(blogKey, index);

            return blogPost.BlavenId;
        }

        public static BlogPost Create(
            string blogKey,
            int index = 0,
            bool isUpdate = false,
            int tagCount = DefaultTagCount)
        {
            var author = new BlogAuthor
                             {
                                 ImageUrl =
                                     TestUtility.GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.ImageUrl),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 Name =
                                     TestUtility.GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.Name),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 SourceId =
                                     TestUtility.GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.SourceId),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 Url =
                                     TestUtility.GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.Url),
                                         blogKey,
                                         index,
                                         isUpdate)
                             };

            var blogPost = new BlogPost
                               {
                                   Author = author,
                                   BlogKey = blogKey,
                                   Content =
                                       TestUtility.GetTestString(nameof(BlogPost.Content), blogKey, index, isUpdate)
                                       + $"{Environment.NewLine}More content",
                                   Hash = TestUtility.GetTestString(nameof(BlogPost.Hash), blogKey, index),
                                   ImageUrl = TestUtility.GetTestString(nameof(BlogPost.ImageUrl), blogKey, index, isUpdate),
                                   PublishedAt = TestPublishedAt.AddDays(index),
                                   SourceId = CreatePostSourceId(index, blogKey),
                                   SourceUrl =
                                       TestUtility.GetTestString(nameof(BlogPost.SourceUrl), blogKey, index, isUpdate),
                                   Summary = TestUtility.GetTestString(nameof(BlogPost.Summary), blogKey, index, isUpdate),
                                   Tags = CreatePostTags(index, tagCount).ToList(),
                                   Title = TestUtility.GetTestString(nameof(BlogPost.Title), blogKey, index, isUpdate),
                                   UpdatedAt = TestUpdatedAt.AddDays(index)
                               };

            blogPost.UrlSlug = BlogSyncConfigurationDefaults.SlugProvider.GetUrlSlug(blogPost);

            blogPost.BlavenId = BlogSyncConfigurationDefaults.BlavenIdProvider.GetBlavenId(blogPost);

            return blogPost;
        }

        public static IReadOnlyList<BlogPost> CreateCollection(
            int start,
            int count,
            string blogKey = BlogMetaTestData.BlogKey)
        {
            var posts = Enumerable.Range(start, count).Select(x => Create(blogKey, x)).ToReadOnlyList();
            return posts;
        }

        public static IReadOnlyList<BlogPost> CreateCollection(
            string blogKey = BlogMetaTestData.BlogKey,
            int blogPostsCount = 11)
        {
            var blogPosts =
                Enumerable.Range(0, blogPostsCount).Select(i => Create(blogKey, i, isUpdate: (i % 2 == 0))).ToList();
            return blogPosts;
        }

        public static string CreatePostSourceId(int index, string blogKey = BlogMetaTestData.BlogKey)
        {
            string postId = $"[{blogKey}]-Post Source Id-{index + 1}";
            return postId;
        }

        public static string CreatePostTag(int index)
        {
            string tag = $"Test Tag {index + 1}";
            return tag;
        }

        public static IEnumerable<string> CreatePostTags(int start, int count)
        {
            var tags = Enumerable.Range(start, count).Select(CreatePostTag);
            return tags;
        }
    }
}
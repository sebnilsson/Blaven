using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Synchronization;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const int DefaultTagCount = 5;

        public static string BlavenId1 => GetBlavenId(1);

        public static string BlavenId2 => GetBlavenId(2);

        public static string BlavenId3 => GetBlavenId(3);

        public static DateTime TestPublishedAt => new DateTime(2015, 1, 1, 12, 30, 45);

        public static DateTime TestUpdatedAt => new DateTime(2015, 2, 2, 14, 45, 30);

        public static string GetBlavenId(int index, string blogKey = BlogKey)
        {
            var blogPost = GetBlogPost(blogKey, index);

            return blogPost.BlavenId;
        }

        public static BlogPost GetBlogPost(
            string blogKey,
            int index = 0,
            bool isUpdate = false,
            int tagCount = DefaultTagCount)
        {
            var author = new BlogAuthor
                             {
                                 ImageUrl =
                                     GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.ImageUrl),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 Name =
                                     GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.Name),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 SourceId =
                                     GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.SourceId),
                                         blogKey,
                                         index,
                                         isUpdate),
                                 Url =
                                     GetTestString(
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
                                       GetTestString(nameof(BlogPost.Content), blogKey, index, isUpdate)
                                       + $"{Environment.NewLine}More content",
                                   Hash = $"HASH_{blogKey}_{index}",
                                   ImageUrl = GetTestString(nameof(BlogPost.ImageUrl), blogKey, index, isUpdate),
                                   PublishedAt = TestPublishedAt.AddDays(index),
                                   SourceId = GetPostSourceId(index, blogKey),
                                   SourceUrl = GetTestString(nameof(BlogPost.SourceUrl), blogKey, index, isUpdate),
                                   Summary = GetTestString(nameof(BlogPost.Summary), blogKey, index, isUpdate),
                                   Tags = GetPostTags(index, tagCount).ToList(),
                                   Title = GetTestString(nameof(BlogPost.Title), blogKey, index, isUpdate),
                                   UpdatedAt = TestUpdatedAt.AddDays(index)
                               };

            blogPost.UrlSlug = BlogSyncConfigurationDefaults.SlugProvider.GetUrlSlug(blogPost);

            blogPost.BlavenId = BlogSyncConfigurationDefaults.BlavenIdProvider.GetBlavenId(blogPost);

            return blogPost;
        }

        public static IEnumerable<BlogPost> GetBlogPosts(int start, int count, string blogKey = BlogKey)
        {
            var posts = Enumerable.Range(start, count).Select(x => GetBlogPost(blogKey, x));
            return posts;
        }

        public static IEnumerable<object[]> GetDbBlogPostsForSingleKey(int start, int count)
        {
            yield return new object[] { GetBlogPosts(start: start, count: count) };
        }

        public static IEnumerable<object[]> GetDbBlogPostsForMultipleKeys(int start, int count)
        {
            var blogPosts1 = GetBlogPosts(start: start, count: count, blogKey: BlogKey);
            var blogPosts2 = GetBlogPosts(start: start, count: count, blogKey: BlogKey2);
            var blogPosts3 = GetBlogPosts(start: start, count: count, blogKey: BlogKey3);

            var blogPosts = blogPosts1.Concat(blogPosts2).Concat(blogPosts3).ToList();
            yield return new object[] { blogPosts };
        }

        public static IEnumerable<object[]> GetDbBlogPostsForSingleAndMultipleKeys(int start, int count)
        {
            foreach (var obj in GetDbBlogPostsForSingleKey(start, count))
            {
                yield return obj;
            }

            foreach (var obj in GetDbBlogPostsForMultipleKeys(start, count))
            {
                yield return obj;
            }
        }

        public static string GetPostTag(int index)
        {
            string tag = $"Test Tag {index + 1}";
            return tag;
        }

        public static IEnumerable<string> GetPostTags(int start, int count)
        {
            var tags = Enumerable.Range(start, count).Select(GetPostTag);
            return tags;
        }

        public static string GetPostSourceId(int index, string blogKey = BlogKey)
        {
            string postId = $"[{blogKey}]-Post Source Id-{index + 1}";
            return postId;
        }
    }
}
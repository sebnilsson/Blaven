using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const int DefaultTagCount = 5;

        public static BlogPost GetBlogPost(
            string blogKey,
            int index,
            int tagCount = DefaultTagCount,
            bool isUpdate = false)
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
                                   Hash = HashUtility.GetBase64(blogKey, index),
                                   ImageUrl = GetTestString(nameof(BlogPost.ImageUrl), blogKey, index, isUpdate),
                                   PublishedAt = TestPublishedAt.AddDays(index),
                                   SourceId = GetPostId(index, blogKey),
                                   SourceUrl =
                                       GetTestString(nameof(BlogPost.SourceUrl), blogKey, index, isUpdate),
                                   Summary = GetTestString(nameof(BlogPost.Summary), blogKey, index, isUpdate),
                                   Tags = GetPostTags(index, tagCount).ToList(),
                                   Title = GetTestString(nameof(BlogPost.Title), blogKey, index, isUpdate),
                                   UrlSlug = GetTestString(nameof(BlogPost.UrlSlug), blogKey, index, isUpdate),
                                   UpdatedAt = TestUpdatedAt.AddDays(index)
                               };

            blogPost.BlavenId = BlavenIdProvider.GetId(blogPost);

            return blogPost;
        }

        public static IEnumerable<BlogPost> GetBlogPosts(int start, int count, string blogKey = BlogKey)
        {
            var posts = Enumerable.Range(start, count).Select(x => GetBlogPost(blogKey, x));
            return posts;
        }

        public static IEnumerable<string> GetPostTags(int start, int count)
        {
            var tags = Enumerable.Range(start, count).Select(i => $"Test Tag {i + 1}");
            return tags;
        }

        public static string GetPostId(int index, string blogKey = BlogKey)
        {
            string postId = $"[{blogKey}]-Post Id-{index + 1}";
            return postId;
        }
    }
}
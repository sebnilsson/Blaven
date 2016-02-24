using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Tests;
using Google.Apis.Blogger.v3.Data;

namespace Blaven.BlogSources.Blogger.Tests
{
    public static class BloggerTestData
    {
        public const string AuthorId = "TestAuthorId";

        public const string AuthorImageUrl = "TestAuthorImageUrl";

        public const string AuthorDisplayName = "TestAuthorDisplayName";

        public const string AuthorUrl = "TestAuthorUrl";

        public const string BlogDescription = "TestBlogDESCRIPTION";

        public const string BlogName = "TestBlogNAME";

        public const string BlogId = "TestBlogID";

        public const string BlogUrl = "TestBlogURL";

        public static readonly DateTime BlogPublishedAt = new DateTime(2016, 1, 1, 12, 1, 1);

        public static readonly DateTime BlogUpdatedAt = new DateTime(2016, 2, 2, 14, 2, 2);

        public const string PostContent = "TestContent\r\nMore content";

        public const string PostId = "TestPostId";

        public static readonly DateTime PostPublishedAt = new DateTime(2016, 3, 3, 15, 3, 3);

        public static readonly IEnumerable<string> PostTags = new[] { "TestTag 1", "TestTag TWO", "TestTag 3" };

        public const string PostTitle = "TestPostTitle";

        public static readonly DateTime PostUpdatedAt = new DateTime(2016, 4, 4, 16, 4, 4);

        public const string PostUrl = "TestPostUrl";

        public static Blog GetBlog()
        {
            var blog = new Blog
                           {
                               Description = BlogDescription,
                               Name = BlogName,
                               Id = BlogId,
                               Published = BlogPublishedAt,
                               Updated = BlogUpdatedAt,
                               Url = BlogUrl
                           };
            return blog;
        }

        public static Post GetPost(
            int index,
            int tagCount = TestData.DefaultTagCount,
            string blogKey = TestData.BlogKey)
        {
            var authorImage = new Post.AuthorData.ImageData
                                  {
                                      Url =
                                          TestData.GetTestString(
                                              $"{nameof(Post)}.{nameof(Post.AuthorData)}.{nameof(Post.AuthorData.ImageData)}.{nameof(Post.AuthorData.ImageData.Url)}",
                                              blogKey,
                                              index)
                                  };
            var author = new Post.AuthorData
                             {
                                 Id =
                                     TestData.GetTestString(
                                         $"{nameof(Post)}.{nameof(Post.AuthorData)}.{nameof(Post.AuthorData.Id)}",
                                         blogKey,
                                         index),
                                 Image = authorImage,
                                 DisplayName =
                                     TestData.GetTestString(
                                         $"{nameof(Post)}.{nameof(Post.AuthorData)}.{nameof(Post.AuthorData.DisplayName)}",
                                         blogKey,
                                         index),
                                 Url =
                                     TestData.GetTestString(
                                         $"{nameof(Post)}.{nameof(Post.AuthorData)}.{nameof(Post.AuthorData.Url)}",
                                         blogKey,
                                         index)
                             };

            var post = new Post
                           {
                               Author = author,
                               Content =
                                   TestData.GetTestString($"{nameof(Post)}.{nameof(Post.Content)}", blogKey, index),
                               Id = TestData.GetPostSourceId(index, blogKey),
                               Labels = TestData.GetPostTags(index, tagCount).ToList(),
                               Published = PostPublishedAt.AddDays(index),
                               Title = TestData.GetTestString($"{nameof(Post)}.{nameof(Post.Title)}", blogKey, index),
                               Url = TestData.GetTestString($"{nameof(Post)}.{nameof(Post.Url)}", blogKey, index),
                               Updated = PostUpdatedAt.AddDays(index)
                           };
            return post;
        }

        public static IEnumerable<Post> GetPosts(int start, int count, string blogKey = TestData.BlogKey)
        {
            var posts = Enumerable.Range(start, count).Select(x => GetPost(x, blogKey: blogKey)).ToList();
            return posts;
        }

        public static IEnumerable<Post> GetPosts()
        {
            var authorImage = new Post.AuthorData.ImageData { Url = AuthorImageUrl };
            var author = new Post.AuthorData
                             {
                                 Id = AuthorId,
                                 Image = authorImage,
                                 DisplayName = AuthorDisplayName,
                                 Url = AuthorUrl
                             };

            var post = new Post
                           {
                               Author = author,
                               Content = PostContent,
                               Id = PostId,
                               Labels = PostTags.ToList(),
                               Published = PostPublishedAt,
                               Title = PostTitle,
                               Url = PostUrl,
                               Updated = BlogUpdatedAt
                           };

            yield return post;
        }
    }
}
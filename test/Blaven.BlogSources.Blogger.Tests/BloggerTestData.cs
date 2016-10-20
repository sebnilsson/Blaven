using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Tests;

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

        public static BloggerBlogData GetBlog()
        {
            var blog = new BloggerBlogData
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

        public static BloggerPostData GetPost(
            int index,
            int tagCount = BlogPostTestData.DefaultTagCount,
            string blogKey = BlogMetaTestData.BlogKey)
        {
            var authorImage = new BloggerPostData.AuthorData.ImageData
                                  {
                                      Url =
                                          TestUtility.GetTestString(
                                              $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.AuthorData)}.{nameof(BloggerPostData.AuthorData.ImageData)}.{nameof(BloggerPostData.AuthorData.ImageData.Url)}",
                                              blogKey,
                                              index)
                                  };
            var author = new BloggerPostData.AuthorData
                             {
                                 Id =
                                     TestUtility.GetTestString(
                                         $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.AuthorData)}.{nameof(BloggerPostData.AuthorData.Id)}",
                                         blogKey,
                                         index),
                                 Image = authorImage,
                                 DisplayName =
                                     TestUtility.GetTestString(
                                         $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.AuthorData)}.{nameof(BloggerPostData.AuthorData.DisplayName)}",
                                         blogKey,
                                         index),
                                 Url =
                                     TestUtility.GetTestString(
                                         $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.AuthorData)}.{nameof(BloggerPostData.AuthorData.Url)}",
                                         blogKey,
                                         index)
                             };

            var post = new BloggerPostData
                           {
                               Author = author,
                               Content =
                                   TestUtility.GetTestString(
                                       $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.Content)}",
                                       blogKey,
                                       index),
                               Id = BlogPostTestData.CreatePostSourceId(index, blogKey),
                               Labels = BlogPostTestData.CreatePostTags(index, tagCount).ToList(),
                               Published = PostPublishedAt.AddDays(index),
                               Title =
                                   TestUtility.GetTestString(
                                       $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.Title)}",
                                       blogKey,
                                       index),
                               Url =
                                   TestUtility.GetTestString(
                                       $"{nameof(BloggerPostData)}.{nameof(BloggerPostData.Url)}",
                                       blogKey,
                                       index),
                               Updated = PostUpdatedAt.AddDays(index)
                           };
            return post;
        }

        public static IEnumerable<BloggerPostData> GetPosts(int start, int count, string blogKey = BlogMetaTestData.BlogKey)
        {
            var posts = Enumerable.Range(start, count).Select(x => GetPost(x, blogKey: blogKey)).ToList();
            return posts;
        }

        public static IEnumerable<BloggerPostData> GetPosts()
        {
            var authorImage = new BloggerPostData.AuthorData.ImageData { Url = AuthorImageUrl };
            var author = new BloggerPostData.AuthorData
                             {
                                 Id = AuthorId,
                                 Image = authorImage,
                                 DisplayName = AuthorDisplayName,
                                 Url = AuthorUrl
                             };

            var post = new BloggerPostData
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
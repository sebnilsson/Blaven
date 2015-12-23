using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static BlogPost GetBlogPost(string blogKey, int index, bool isUpdate)
        {
            var author = new BlogAuthor
                             {
                                 Id =
                                     GetTestString(
                                         nameof(BlogPost.Author) + nameof(BlogAuthor.Id),
                                         blogKey,
                                         index,
                                         isUpdate),
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
                                   SourceId = GetTestString(nameof(BlogPost.SourceId), blogKey, index, isUpdate),
                                   SourceUrl =
                                       GetTestString(nameof(BlogPost.SourceUrl), blogKey, index, isUpdate),
                                   Summary = GetTestString(nameof(BlogPost.Summary), blogKey, index, isUpdate),
                                   Tags = GetTestBlogPostTags(5),
                                   Title = GetTestString(nameof(BlogPost.Title), blogKey, index, isUpdate),
                                   UrlSlug = GetTestString(nameof(BlogPost.UrlSlug), blogKey, index, isUpdate),
                                   UpdatedAt = TestUpdatedAt.AddDays(index)
                               };

            blogPost.BlavenId = BlavenIdProvider.GetId(blogPost);

            return blogPost;
        }

        internal static string GetTestString(string name, string blogKey, int index, bool isUpdate)
        {
            string prefix = isUpdate ? "Updated" : null;

            string testString = $"{prefix}Test{name}_{blogKey}_{index}";
            return testString;
        }

        private static IEnumerable<string> GetTestBlogPostTags(int tagCount)
        {
            var tags = Enumerable.Range(0, tagCount).Select(i => $"TestTag_{i}").ToList();
            return tags;
        }
    }
}
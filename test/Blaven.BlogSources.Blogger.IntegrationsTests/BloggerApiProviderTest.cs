using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiProviderTest
    {
        [Theory]
        [MemberData(nameof(AfterLastUpdatedAtData))]
        [MemberData(nameof(ForceUpdateData))]
        public void GetPosts_BloggerBlogContainsData_ReturnsAllPostsFromAllPages(
            DateTime lastUpdatedAt,
            int expectedPostCount)
        {
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 5);
            var blogSettings = GetTestBlogSetting();

            var posts = bloggerApiProvider.GetPosts(blogSettings.Id, lastUpdatedAt).ToList();

            Assert.Equal(expectedPostCount, posts.Count);
        }

        [Fact]
        public void GetPosts_BloggerBlogContainsData_ReturnsAllPostFields()
        {
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 2);
            var blogSettings = GetTestBlogSetting();

            var posts = bloggerApiProvider.GetPosts(blogSettings.Id, DateTime.MinValue).Take(2).ToList();

            var firstPost = posts.FirstOrDefault();

            Assert.NotNull(firstPost);
            Assert.NotNull(firstPost.Author);
            Assert.NotNull(firstPost.Author.DisplayName);
            Assert.NotNull(firstPost.Author.Id);
            Assert.NotNull(firstPost.Author.Image);
            Assert.NotNull(firstPost.Author.Image.Url);
            Assert.NotNull(firstPost.Author.Url);
            Assert.NotNull(firstPost.Content);
            Assert.NotNull(firstPost.Id);
            Assert.NotNull(firstPost.Labels);
            var labelsContainsElements = firstPost.Labels.Any();
            Assert.True(labelsContainsElements);
            Assert.NotNull(firstPost.Published);
            Assert.NotNull(firstPost.Title);
            Assert.NotNull(firstPost.Updated);
            Assert.NotNull(firstPost.Url);
        }

        public static IEnumerable<object> ForceUpdateData()
        {
            yield return new object[] { DateTime.MinValue, AppSettingsHelper.ExpectedPostsCount };
        }

        public static IEnumerable<object> AfterLastUpdatedAtData()
        {
            yield return
                new object[]
                    {
                        AppSettingsHelper.ExpectedAfterLastUpdatedAt,
                        AppSettingsHelper.ExpectedAfterLastUpdatedAtPostsCount
                    };
        }

        private static BloggerApiProvider GetTestBloggerApiProvider(
            int postListRequestMaxResults = BloggerApiProvider.DefaultPostListRequestMaxResults)
        {
            string apiKey = AppSettingsHelper.GetApiKey();

            var bloggerApiProvider = new BloggerApiProvider(apiKey, postListRequestMaxResults);
            return bloggerApiProvider;
        }

        private static BlogSetting GetTestBlogSetting()
        {
            string blogId = AppSettingsHelper.GetBlogId();

            var settings = new BlogSetting(string.Empty, blogId, string.Empty);
            return settings;
        }
    }
}
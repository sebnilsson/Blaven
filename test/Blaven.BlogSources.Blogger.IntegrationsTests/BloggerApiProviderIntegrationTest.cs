using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Xunit;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiProviderIntegrationTest
    {
        private const string ApiKeyAppSettingsKey = "Blaven.BlogSources.Blogger.ApiKey";

        private const string BlogIdAppSettingsKey = "Blaven.Blogs.TestBlog.Id";

        private const string BloggerExpectedPostsCountAppSettingsKey = "Blaven.BlogSources.Blogger.ExpectedPostsCount";

        private static readonly int ExpectedPostsCount = GetExpectedPostsCount();

        [Fact]
        public void GetPosts_BloggerBlogContainsData_ReturnsAllPostsFromAllPages()
        {
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 5);
            var blogSettings = GetTestBlogSetting();

            var posts = bloggerApiProvider.GetPosts(blogSettings.Id).ToList();

            Assert.Equal(ExpectedPostsCount, posts.Count);
        }

        [Fact]
        public void GetPosts_BloggerBlogContainsData_ReturnsAllPostFields()
        {
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 2);
            var blogSettings = GetTestBlogSetting();

            var posts = bloggerApiProvider.GetPosts(blogSettings.Id).Take(2).ToList();

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

        private static int GetExpectedPostsCount()
        {
            string expectedPostsCountValue = ConfigurationManager.AppSettings[BloggerExpectedPostsCountAppSettingsKey];
            if (string.IsNullOrWhiteSpace(expectedPostsCountValue))
            {
                throw new KeyNotFoundException(
                    $"No AppSetting-value found for key '{BloggerExpectedPostsCountAppSettingsKey}'.");
            }

            int expectedPostsCount;
            if (!int.TryParse(expectedPostsCountValue, out expectedPostsCount))
            {
                throw new KeyNotFoundException(
                    $"Could not parse AppSetting-value to int for key '{BloggerExpectedPostsCountAppSettingsKey}'.");
            }

            return expectedPostsCount;
        }

        private static BloggerApiProvider GetTestBloggerApiProvider(
            int postListRequestMaxResults = BloggerApiProvider.DefaultPostListRequestMaxResults)
        {
            string apiKey = ConfigurationManager.AppSettings[ApiKeyAppSettingsKey];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new KeyNotFoundException($"No AppSetting-value found for key '{ApiKeyAppSettingsKey}'.");
            }

            var bloggerApiProvider = new BloggerApiProvider(apiKey, postListRequestMaxResults);
            return bloggerApiProvider;
        }

        private static BlogSetting GetTestBlogSetting()
        {
            string blogId = ConfigurationManager.AppSettings[BlogIdAppSettingsKey];
            if (string.IsNullOrWhiteSpace(blogId))
            {
                throw new KeyNotFoundException($"No AppSetting-value found for key '{BlogIdAppSettingsKey}'.");
            }

            var settings = new BlogSetting(string.Empty, blogId, string.Empty);
            return settings;
        }
    }
}
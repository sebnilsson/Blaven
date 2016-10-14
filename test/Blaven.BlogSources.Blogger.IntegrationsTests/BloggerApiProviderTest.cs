using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiProviderTest
    {


        [Fact]
        public async Task GetBlog_BloggerBlogContainsData_ReturnsWithAllFields()
        {
            // Arrange
            var bloggerApiProvider = GetTestBloggerApiProvider();
            var blogSetting = GetTestBlogSetting();
            
            // Act
            var blog = await bloggerApiProvider.GetBlog(blogSetting.Id);

            // Assert
            Assert.NotNull(blog);
            Assert.NotNull(blog.Description);
            Assert.NotNull(blog.Id);
            Assert.NotNull(blog.Name);
            Assert.NotNull(blog.Published);
            Assert.NotNull(blog.Updated);
            Assert.NotNull(blog.Url);
        }

        [Theory]
        [MemberData(nameof(AfterLastUpdatedAtData))]
        [MemberData(nameof(ForceUpdateData))]
        public async Task GetPosts_BloggerBlogContainsData_ReturnsAllPostsFromAllPages(
            DateTime lastUpdatedAt,
            int expectedPostCount)
        {
            // Arrange
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 5);
            var blogSetting = GetTestBlogSetting();

            // Act
            var posts = await bloggerApiProvider.GetPosts(blogSetting.Id, lastUpdatedAt);

            // Assert
            Assert.Equal(expectedPostCount, posts.Count);
        }

        [Fact]
        public async Task GetPosts_BloggerBlogContainsData_ReturnsPostWithAllFields()
        {
            // Arrange
            var bloggerApiProvider = GetTestBloggerApiProvider(postListRequestMaxResults: 2);
            var blogSetting = GetTestBlogSetting();

            // Act
            var posts = await bloggerApiProvider.GetPosts(blogSetting.Id, DateTime.MinValue);

            // Assert
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

            var bloggerApiProvider = new BloggerApiProvider(apiKey)
                                         {
                                             PostListRequestMaxResults = postListRequestMaxResults
                                         };

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
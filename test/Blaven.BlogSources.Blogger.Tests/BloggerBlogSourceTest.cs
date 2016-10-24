using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Tests;
using Xunit;

namespace Blaven.BlogSources.Blogger.Tests
{
    public class BloggerBlogSourceTest
    {
        private const string TestBlogId = "TestBlogId";

        private const string TestBlogName = "TestBlogName";

        private static readonly DateTime TestLastUpdatedAt = new DateTime(2015, 1, 1);

        [Fact]
        public async Task GetMeta_BloggerApiProviderContainsData_ShouldReturnAllFields()
        {
            // Arrange
            var bloggerBlogSource = GetTestBloggerBlogSource(getBlogFunc: _ => BloggerTestData.GetBlog());
            var blogSetting = GetTestBlogSetting();

            // Act
            var meta = await bloggerBlogSource.GetMeta(blogSetting, TestLastUpdatedAt);

            // Assert
            Assert.NotNull(meta);
            Assert.Equal(blogSetting.BlogKey, meta.BlogKey);
            Assert.Equal(BloggerTestData.BlogDescription, meta.Description);
            Assert.Equal(BloggerTestData.BlogName, meta.Name);
            Assert.Equal(BloggerTestData.BlogPublishedAt, meta.PublishedAt);
            Assert.Equal(BloggerTestData.BlogId, meta.SourceId);
            Assert.Equal(BloggerTestData.BlogUpdatedAt, meta.UpdatedAt);
            Assert.Equal(BloggerTestData.BlogUrl, meta.Url);
        }

        [Fact]
        public async Task GetBlogPosts_BloggerApiProviderContainsDataAndEmptyDb_ReturnsInsertWithAllFields()
        {
            // Arrange
            var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts());
            var blogSetting = GetTestBlogSetting();

            // Act
            var blogPosts = await bloggerBlogSource.GetBlogPosts(blogSetting, TestLastUpdatedAt);

            // Assert
            var post = blogPosts.FirstOrDefault();

            Assert.NotNull(post);
            Assert.Equal(blogSetting.BlogKey, post.BlogKey);
            Assert.Equal(BloggerTestData.AuthorId, post.Author.SourceId);
            Assert.Equal(BloggerTestData.AuthorImageUrl, post.Author.ImageUrl);
            Assert.Equal(BloggerTestData.AuthorDisplayName, post.Author.Name);
            Assert.Equal(BloggerTestData.AuthorUrl, post.Author.Url);
            Assert.Equal(BloggerTestData.PostContent, post.Content);
            Assert.Equal(BloggerTestData.PostId, post.SourceId);
            Assert.Equal(BloggerTestData.PostTitle, post.Title);
            Assert.Equal(BloggerTestData.PostUrl, post.SourceUrl);
            Assert.Equal(BloggerTestData.PostPublishedAt, post.PublishedAt);
            bool tagsAreSequenceEquals =
                BloggerTestData.PostTags.OrderBy(x => x).SequenceEqual(post.Tags.OrderBy(x => x));
            Assert.True(tagsAreSequenceEquals);
        }

        private static BloggerBlogSource GetTestBloggerBlogSource(
            Func<string, BloggerBlogData> getBlogFunc = null,
            Func<string, IEnumerable<BloggerPostData>> getPostsFunc = null)
        {
            var bloggerApiProvider = new MockBloggerApiProvider(getBlogFunc, getPostsFunc);

            var bloggerBlogSource = new BloggerBlogSource(bloggerApiProvider);
            return bloggerBlogSource;
        }

        private static BlogSetting GetTestBlogSetting()
        {
            var blogSettings = new BlogSetting(BlogMetaTestData.BlogKey, TestBlogId, TestBlogName);
            return blogSettings;
        }
    }
}
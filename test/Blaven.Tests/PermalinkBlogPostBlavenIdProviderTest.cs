using System;
using Xunit;

namespace Blaven.Tests
{
    public class PermalinkBlogPostBlavenIdProviderTest
    {
        private const string TestUrlSlug = "nullableguidconstraint-for-asp-net-mvc-webapi";
        private static readonly DateTime TestPublishedAt = new DateTime(2015, 3, 6);

        [Fact]
        public void GetBlavenId_IncludePublishedYearMonth_ReturnsBlavenIdIncludingYearMonth()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(true, false);
            var blogPost = new BlogPost
                           {
                               UrlSlug = TestUrlSlug,
                               PublishedAt = TestPublishedAt
                           };

            // Act
            var blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal("2015/03/nullableguidconstraint-for-asp-net-mvc-webapi", blavenId);
        }

        [Fact]
        public void GetBlavenId_IncludePublishedYearMonthAndDay_ReturnsBlavenIdIncludingYearMonthAndDay()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(true, true);
            var blogPost = new BlogPost
                           {
                               UrlSlug = TestUrlSlug,
                               PublishedAt = TestPublishedAt
                           };

            // Act
            var blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal("2015/03/06/nullableguidconstraint-for-asp-net-mvc-webapi", blavenId);
        }

        [Fact]
        public void GetBlavenId_NoInclude_ReturnsUrlSlug()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(false, false);
            var blogPost = new BlogPost
                           {
                               UrlSlug = TestUrlSlug,
                               PublishedAt = TestPublishedAt
                           };

            // Act
            var blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal(TestUrlSlug, blavenId);
        }
    }
}
using System;

using Xunit;

namespace Blaven.Tests
{
    public class PermalinkBlogPostBlavenIdProviderTest
    {
        private static readonly DateTime TestPublishedAt = new DateTime(2015, 3, 6);

        private const string TestUrlSlug = "nullableguidconstraint-for-asp-net-mvc-webapi";

        [Fact]
        public void GetBlavenId_IncludePublishedYearMonthAndDay_ReturnsBlavenIdIncludingYearMonthAndDay()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(
                includePublishedYearAndMonth: true,
                includePublishedDay: true);
            var blogPost = new BlogPost { UrlSlug = TestUrlSlug, PublishedAt = TestPublishedAt };

            // Act
            string blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal("2015/03/06/nullableguidconstraint-for-asp-net-mvc-webapi", blavenId);
        }

        [Fact]
        public void GetBlavenId_IncludePublishedYearMonth_ReturnsBlavenIdIncludingYearMonth()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(
                includePublishedYearAndMonth: true,
                includePublishedDay: false);
            var blogPost = new BlogPost { UrlSlug = TestUrlSlug, PublishedAt = TestPublishedAt };

            // Act
            string blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal("2015/03/nullableguidconstraint-for-asp-net-mvc-webapi", blavenId);
        }

        [Fact]
        public void GetBlavenId_NoInclude_ReturnsUrlSlug()
        {
            // Arrange
            var provider = new PermalinkBlogPostBlavenIdProvider(
                includePublishedYearAndMonth: false,
                includePublishedDay: false);
            var blogPost = new BlogPost { UrlSlug = TestUrlSlug, PublishedAt = TestPublishedAt };

            // Act
            string blavenId = provider.GetBlavenId(blogPost);

            // Assert
            Assert.Equal(TestUrlSlug, blavenId);
        }
    }
}
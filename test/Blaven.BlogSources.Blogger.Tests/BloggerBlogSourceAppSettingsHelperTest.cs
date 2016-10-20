using System.Collections.Generic;

using Blaven.Tests;
using Xunit;

namespace Blaven.BlogSources.Blogger.Tests
{
    public class BloggerBlogSourceAppSettingsHelperTest
    {
        [Fact]
        public void CreateFromAppSettingsInternal_AppSettingsContainingApiKey_ReturnsBloggerBlogSource()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();

            // Act
            var bloggerBlogSource = BloggerBlogSourceAppSettingsHelper.CreateFromAppSettingsInternal(appSettings);

            // Assert
            Assert.NotNull(bloggerBlogSource);
        }

        [Fact]
        public void CreateFromAppSettingsInternal_EmptyAppSettings_ThrowsException()
        {
            // Arrange
            var appSettings = new Dictionary<string, string>(0);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(
                () => BloggerBlogSourceAppSettingsHelper.CreateFromAppSettingsInternal(appSettings));
        }
    }
}
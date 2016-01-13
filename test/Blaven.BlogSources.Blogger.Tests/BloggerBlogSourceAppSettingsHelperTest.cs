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
            var appSettings = TestData.GetAppSettings();

            var bloggerBlogSource = BloggerBlogSourceAppSettingsHelper.CreateFromAppSettingsInternal(appSettings);

            Assert.NotNull(bloggerBlogSource);
        }

        [Fact]
        public void CreateFromAppSettingsInternal_EmptyAppSettings_ThrowsException()
        {
            var appSettings = new Dictionary<string, string>(0);

            Assert.Throws<KeyNotFoundException>(
                () => BloggerBlogSourceAppSettingsHelper.CreateFromAppSettingsInternal(appSettings));
        }
    }
}
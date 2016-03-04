using System;
using System.Linq;

using Blaven.Tests;
using Xunit;

namespace Blaven.BlogSources.Tests
{
    public class BlogSourceFactoryTest
    {
        [Fact]
        public void BuildBlogSourceInternal_NullMockBlogSource_FuncContainsUsernameAndPassword()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            string username = null;
            string password = null;

            // Act
            AppSettingsFactory.BuildBlogSourceInternal<MockBlogSource>(
                (appSettingUsername, appSettingsPassword) =>
                    {
                        username = appSettingUsername;
                        password = appSettingsPassword;
                        return null;
                    },
                appSettings);

            // Assert
            Assert.Equal(TestData.AppSettingsTestUsername, username);
            Assert.Equal(TestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void BuildBlogSourceInternal_MockBlogSource_ReturnsNotNullBlogSource()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            // Act
            var blogSource = AppSettingsFactory.BuildBlogSourceInternal((_, __) => MockBlogSource.Create(), appSettings);

            // Assert
            Assert.NotNull(blogSource);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettings()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            string blogKey1 = nameof(TestData.BlogKey1).ToLowerInvariant();
            string blogKey2 = nameof(TestData.BlogKey2).ToLowerInvariant();

            // Act
            var blogSettings = AppSettingsFactory.GetBlogSettingsInternal(appSettings).ToList();

            // Assert
            var blogSetting1 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey1);
            var blogSetting2 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey2);

            Assert.NotNull(blogSetting1);
            Assert.Equal(blogKey1, blogSetting1.BlogKey);
            Assert.Equal($"{nameof(TestData.BlogKey1)}Id", blogSetting1.Id);
            Assert.Equal($"{nameof(TestData.BlogKey1)}Name", blogSetting1.Name);

            Assert.NotNull(blogSetting2);
            Assert.Equal(blogKey2, blogSetting2.BlogKey);
            Assert.Equal($"{nameof(TestData.BlogKey2)}Id", blogSetting2.Id);
            Assert.Equal($"{nameof(TestData.BlogKey2)}Name", blogSetting2.Name);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettingsWithoutIdOrName()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            string blogKey3 = nameof(TestData.BlogKey3).ToLowerInvariant();

            // Act
            var blogSettings = AppSettingsFactory.GetBlogSettingsInternal(appSettings).ToList();

            // Assert
            var blogSetting3 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey3);

            Assert.NotNull(blogSetting3);
            Assert.Equal(blogKey3, blogSetting3.BlogKey);
            Assert.Null(blogSetting3.Id);
            Assert.Null(blogSetting3.Name);
        }
    }
}
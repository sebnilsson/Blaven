using System.Linq;
using Blaven.BlogSources.Testing;
using Blaven.Configuration;
using Blaven.Testing;
using Xunit;

namespace Blaven.BlogSources.Configuration.Tests
{
    public class BlogSourceFactoryTest
    {
        [Fact]
        public void BuildBlogSource_MockBlogSource_ReturnsNotNullBlogSource()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();
            var service = new AppSettingsConfigService(appSettings);

            // Act
            var blogSource = service.BuildBlogSource((_, __) => new FakeBlogSource());

            // Assert
            Assert.NotNull(blogSource);
        }

        [Fact]
        public void BuildBlogSource_NullMockBlogSource_FuncContainsUsernameAndPassword()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();
            var service = new AppSettingsConfigService(appSettings);

            string username = null;
            string password = null;

            // Act
            service.BuildBlogSource<FakeBlogSource>(
                (appSettingUsername, appSettingsPassword) =>
                {
                    username = appSettingUsername;
                    password = appSettingsPassword;
                    return null;
                });

            // Assert
            Assert.Equal(AppSettingTestData.AppSettingsTestUsername, username);
            Assert.Equal(AppSettingTestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettings()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();
            var service = new AppSettingsConfigService(appSettings);

            var blogKey1 = nameof(BlogMetaTestData.BlogKey1).ToLowerInvariant();
            var blogKey2 = nameof(BlogMetaTestData.BlogKey2).ToLowerInvariant();

            // Act
            var blogSettings = service.GetBlogSettings().ToList();

            // Assert
            var blogSetting1 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey1);
            var blogSetting2 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey2);

            Assert.NotNull(blogSetting1);
            Assert.Equal(blogKey1, blogSetting1.BlogKey);
            Assert.Equal($"{nameof(BlogMetaTestData.BlogKey1)}Id", blogSetting1.Id);
            Assert.Equal($"{nameof(BlogMetaTestData.BlogKey1)}Name", blogSetting1.Name);

            Assert.NotNull(blogSetting2);
            Assert.Equal(blogKey2, blogSetting2.BlogKey);
            Assert.Equal($"{nameof(BlogMetaTestData.BlogKey2)}Id", blogSetting2.Id);
            Assert.Equal($"{nameof(BlogMetaTestData.BlogKey2)}Name", blogSetting2.Name);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettingsWithoutIdOrName()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();
            var service = new AppSettingsConfigService(appSettings);

            var blogKey3 = nameof(BlogMetaTestData.BlogKey3).ToLowerInvariant();

            // Act
            var blogSettings = service.GetBlogSettings().ToList();

            // Assert
            var blogSetting3 = blogSettings.FirstOrDefault(x => x.BlogKey == blogKey3);

            Assert.NotNull(blogSetting3);
            Assert.Equal(blogKey3, blogSetting3.BlogKey);
            Assert.Null(blogSetting3.Id);
            Assert.Null(blogSetting3.Name);
        }
    }
}
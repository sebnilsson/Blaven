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
            var appSettings = TestData.GetAppSettings();

            string username = null;
            string password = null;

            AppSettingsFactory.BuildBlogSourceInternal<MockBlogSource>(
                (appSettingUsername, appSettingsPassword) =>
                    {
                        username = appSettingUsername;
                        password = appSettingsPassword;
                        return null;
                    },
                appSettings);

            Assert.Equal(TestData.AppSettingsTestUsername, username);
            Assert.Equal(TestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void BuildBlogSourceInternal_MockBlogSource_ReturnsNotNullBlogSource()
        {
            var appSettings = TestData.GetAppSettings();

            var blogSource = AppSettingsFactory.BuildBlogSourceInternal((_, __) => MockBlogSource.Create(), appSettings);

            Assert.NotNull(blogSource);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettings()
        {
            var appSettings = TestData.GetAppSettings();

            var blogSettings = AppSettingsFactory.GetBlogSettingsInternal(appSettings).ToList();

            var blogSetting1 = blogSettings.FirstOrDefault(x => x.BlogKey == nameof(TestData.BlogKey1));
            var blogSetting2 = blogSettings.FirstOrDefault(x => x.BlogKey == nameof(TestData.BlogKey2));

            Assert.NotNull(blogSetting1);
            Assert.Equal(nameof(TestData.BlogKey1), blogSetting1.BlogKey);
            Assert.Equal($"{nameof(TestData.BlogKey1)}Id", blogSetting1.Id);
            Assert.Equal($"{nameof(TestData.BlogKey1)}Name", blogSetting1.Name);

            Assert.NotNull(blogSetting2);
            Assert.Equal(nameof(TestData.BlogKey2), blogSetting2.BlogKey);
            Assert.Equal($"{nameof(TestData.BlogKey2)}Id", blogSetting2.Id);
            Assert.Equal($"{nameof(TestData.BlogKey2)}Name", blogSetting2.Name);
        }

        [Fact]
        public void GetBlogSettingsInternal_AppSettingsContainingSettings_ReturnsBlogSettingsWithoutIdOrName()
        {
            var appSettings = TestData.GetAppSettings();

            var blogSettings = AppSettingsFactory.GetBlogSettingsInternal(appSettings).ToList();

            var blogSetting3 = blogSettings.FirstOrDefault(x => x.BlogKey == nameof(TestData.BlogKey3));

            Assert.NotNull(blogSetting3);
            Assert.Equal(nameof(TestData.BlogKey3), blogSetting3.BlogKey);
            Assert.Null(blogSetting3.Id);
            Assert.Null(blogSetting3.Name);
        }
    }
}
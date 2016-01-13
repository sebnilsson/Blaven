using System.Collections.Generic;

using Blaven.Tests;
using Xunit;

namespace Blaven.BlogSources.Tests
{
    public class BlogSourceAppSettingsUtilityTest
    {
        [Fact]
        public void GetPasswordInternal_PasswordAndMockBlogSource_ReturnsUserName()
        {
            var appSettings = TestData.GetAppSettings();

            string password = BlogSourceAppSettingsUtility.GetPasswordInternal<MockBlogSource>(appSettings);

            Assert.Equal(TestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetUsernameInternal_UsernameAndMockBlogSource_ReturnsUsername()
        {
            var appSettings = TestData.GetAppSettings();

            string username = BlogSourceAppSettingsUtility.GetUsernameInternal<MockBlogSource>(appSettings);

            Assert.Equal(TestData.AppSettingsTestUsername, username);
        }

        [Fact]
        public void GetValueInternal_NonExistingKeyAndMockBlogSource_Throws()
        {
            var appSettings = TestData.GetAppSettings();

            Assert.Throws<KeyNotFoundException>(
                () => BlogSourceAppSettingsUtility.GetValueInternal<MockBlogSource>("NonExistingKey", appSettings));
        }
    }
}
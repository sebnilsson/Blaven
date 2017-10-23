using System.Collections.Generic;
using Blaven.BlogSources.Testing;
using Blaven.Testing;
using Xunit;

namespace Blaven.Configuration.Tests
{
    public class BlogSourceAppSettingsUtilityTest
    {
        [Fact]
        public void GetPassword_PasswordAndMockBlogSource_ReturnsUserName()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();

            // Act
            var password = AppSettingsUtility.GetPassword<FakeBlogSource>(appSettings);

            // Assert
            Assert.Equal(AppSettingTestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetUsername_UsernameAndMockBlogSource_ReturnsUsername()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();

            // Act
            var username = AppSettingsUtility.GetUsername<FakeBlogSource>(appSettings);

            // Assert
            Assert.Equal(AppSettingTestData.AppSettingsTestUsername, username);
        }

        [Fact]
        public void GetValueInternal_NonExistingKeyAndMockBlogSource_Throws()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(
                () => AppSettingsUtility.GetValue<FakeBlogSource>("NonExistingKey", appSettings));
        }
    }
}
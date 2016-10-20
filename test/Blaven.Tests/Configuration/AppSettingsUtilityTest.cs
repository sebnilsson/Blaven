using System.Collections.Generic;

using Blaven.BlogSources.Tests;
using Blaven.Tests;
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
            string password = AppSettingsUtility.GetPassword<MockBlogSource>(appSettings);

            // Assert
            Assert.Equal(AppSettingTestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetUsername_UsernameAndMockBlogSource_ReturnsUsername()
        {
            // Arrange
            var appSettings = AppSettingTestData.CreateDictionary();

            // Act
            string username = AppSettingsUtility.GetUsername<MockBlogSource>(appSettings);

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
                () => AppSettingsUtility.GetValue<MockBlogSource>("NonExistingKey", appSettings));
        }
    }
}
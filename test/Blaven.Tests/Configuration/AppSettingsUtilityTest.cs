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
            var appSettings = TestData.GetAppSettings();

            // Act
            string password = AppSettingsUtility.GetPassword<MockBlogSource>(appSettings);

            // Assert
            Assert.Equal(TestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetUsername_UsernameAndMockBlogSource_ReturnsUsername()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            // Act
            string username = AppSettingsUtility.GetUsername<MockBlogSource>(appSettings);

            // Assert
            Assert.Equal(TestData.AppSettingsTestUsername, username);
        }

        [Fact]
        public void GetValueInternal_NonExistingKeyAndMockBlogSource_Throws()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(
                () => AppSettingsUtility.GetValue<MockBlogSource>("NonExistingKey", appSettings));
        }
    }
}
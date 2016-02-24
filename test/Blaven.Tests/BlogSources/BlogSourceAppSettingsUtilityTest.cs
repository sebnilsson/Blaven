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
            // Arrange
            var appSettings = TestData.GetAppSettings();

            // Act
            string password = BlogSourceAppSettingsUtility.GetPasswordInternal<MockBlogSource>(appSettings);

            // Assert
            Assert.Equal(TestData.AppSettingsTestPassword, password);
        }

        [Fact]
        public void GetUsernameInternal_UsernameAndMockBlogSource_ReturnsUsername()
        {
            // Arrange
            var appSettings = TestData.GetAppSettings();

            // Act
            string username = BlogSourceAppSettingsUtility.GetUsernameInternal<MockBlogSource>(appSettings);

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
                () => BlogSourceAppSettingsUtility.GetValueInternal<MockBlogSource>("NonExistingKey", appSettings));
        }
    }
}
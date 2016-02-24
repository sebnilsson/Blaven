using System.Linq;

using Xunit;

namespace Blaven.Tests
{
    public class BlogSettingsManagerTest
    {
        [Fact]
        public void GetEnsuredBlogKey_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            // Arrange
            var settingsManager = TestData.GetTestBlogSettingsManager();

            // Act
            var ensuredBlogKey = settingsManager.GetEnsuredBlogKey(TestData.BlogKey);

            // Assert
            Assert.Equal(TestData.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKey_NullBlogKey_ReturnsFirstBlogSettingsBlogKey()
        {
            // Arrange
            var settingsManager = TestData.GetTestBlogSettingsManager();

            // Act
            var ensuredBlogKey = settingsManager.GetEnsuredBlogKey(blogKey: null);

            // Assert
            var firstBlogSetting = TestData.GetBlogSettings().FirstOrDefault();

            Assert.NotNull(ensuredBlogKey);
            Assert.NotNull(firstBlogSetting);
            Assert.Equal(firstBlogSetting.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            // Arrange
            var settingsManager = TestData.GetTestBlogSettingsManager();
            var blogKeys = new[] { TestData.BlogKey };

            // Act
            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys);

            // Assert
            bool ensuredBlogKeysSequenceEqualsBlogKeys = ensuredBlogKeys.SequenceEqual(blogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_EmptyBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            // Arrange
            var settingsManager = TestData.GetTestBlogSettingsManager();

            // Act
            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys: Enumerable.Empty<string>());

            // Assert
            var blogSettingsBlogKeys = TestData.GetBlogSettings().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NullBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            // Arrange
            var settingsManager = TestData.GetTestBlogSettingsManager();

            // Act
            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys: null);

            // Assert
            var blogSettingsBlogKeys = TestData.GetBlogSettings().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }
    }
}
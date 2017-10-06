using System.Collections.Generic;
using System.Linq;

using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class BlogSettingsHelperTest
    {
        [Fact]
        public void GetEnsuredBlogKey_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            // Arrange
            var settingsHelper = GetTestBlogSettingsHelper();

            // Act
            var ensuredBlogKey = settingsHelper.GetEnsuredBlogKey(BlogMetaTestData.BlogKey);

            // Assert
            Assert.Equal(BlogMetaTestData.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKey_NullBlogKey_ReturnsFirstBlogSettingsBlogKey()
        {
            // Arrange
            var settingsHelper = GetTestBlogSettingsHelper();

            // Act
            var ensuredBlogKey = settingsHelper.GetEnsuredBlogKey(blogKey: null);

            // Assert
            var firstBlogSetting = BlogSettingTestData.CreateCollection().FirstOrDefault();

            Assert.NotNull(ensuredBlogKey);
            Assert.NotNull(firstBlogSetting);
            Assert.Equal(firstBlogSetting.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            // Arrange
            var settingsHelper = GetTestBlogSettingsHelper();
            var blogKeys = new[] { BlogMetaTestData.BlogKey };

            // Act
            var ensuredBlogKeys = settingsHelper.GetEnsuredBlogKeys(blogKeys);

            // Assert
            bool ensuredBlogKeysSequenceEqualsBlogKeys = ensuredBlogKeys.SequenceEqual(blogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_EmptyBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            // Arrange
            var settingsHelper = GetTestBlogSettingsHelper();

            // Act
            var ensuredBlogKeys = settingsHelper.GetEnsuredBlogKeys(blogKeys: Enumerable.Empty<string>());

            // Assert
            var blogSettingsBlogKeys = BlogSettingTestData.CreateCollection().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NullBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            // Arrange
            var settingsHelper = GetTestBlogSettingsHelper();

            // Act
            var ensuredBlogKeys = settingsHelper.GetEnsuredBlogKeys(blogKeys: (IEnumerable<string>)null);

            // Assert
            var blogSettingsBlogKeys = BlogSettingTestData.CreateCollection().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }

        private static BlogSettingsHelper GetTestBlogSettingsHelper()
        {
            var settings = BlogSettingTestData.CreateCollection();

            var helper = new BlogSettingsHelper(settings);
            return helper;
        }
    }
}
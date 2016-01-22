using System.Linq;

using Xunit;

namespace Blaven.Tests
{
    public class BlogSettingsManagerTest
    {
        [Fact]
        public void GetEnsuredBlogKey_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            var settingsManager = TestData.GetTestBlogSettingsManager();

            var ensuredBlogKey = settingsManager.GetEnsuredBlogKey(TestData.BlogKey);

            Assert.Equal(TestData.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKey_NullBlogKey_ReturnsFirstBlogSettingsBlogKey()
        {
            var settingsManager = TestData.GetTestBlogSettingsManager();

            var ensuredBlogKey = settingsManager.GetEnsuredBlogKey(blogKey: null);

            var firstBlogSetting = TestData.GetBlogSettings().FirstOrDefault();

            Assert.NotNull(ensuredBlogKey);
            Assert.NotNull(firstBlogSetting);
            Assert.Equal(firstBlogSetting.BlogKey, ensuredBlogKey);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NonExistingBlogKey_ReturnsNonExistingBlogKey()
        {
            var settingsManager = TestData.GetTestBlogSettingsManager();
            var blogKeys = new[] { TestData.BlogKey };

            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys);

            bool ensuredBlogKeysSequenceEqualsBlogKeys = ensuredBlogKeys.SequenceEqual(blogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_EmptyBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            var settingsManager = TestData.GetTestBlogSettingsManager();

            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys: Enumerable.Empty<string>());

            var blogSettingsBlogKeys = TestData.GetBlogSettings().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }

        [Fact]
        public void GetEnsuredBlogKeys_NullBlogKeys_ReturnsAllBlogSettingsBlogKeys()
        {
            var settingsManager = TestData.GetTestBlogSettingsManager();

            var ensuredBlogKeys = settingsManager.GetEnsuredBlogKeys(blogKeys: null);

            var blogSettingsBlogKeys = TestData.GetBlogSettings().Select(x => x.BlogKey);

            bool ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys = ensuredBlogKeys.SequenceEqual(blogSettingsBlogKeys);
            Assert.True(ensuredBlogKeysSequenceEqualsBlogSettingBlogKeys);
        }
    }
}
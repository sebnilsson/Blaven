using System.Collections.Generic;

namespace Blaven.Testing
{
    public static class BlogSettingTestData
    {
        public const string BlogSettingsId = "TestSettingsId";

        public const string BlogSettingsId1 = "TestSettingsId 1";

        public const string BlogSettingsId2 = "TestSettingsId 2";

        public const string BlogSettingsId3 = "TestSettingsId 3";

        public const string BlogSettingsName = "TestSettingsName";

        public const string BlogSettingsName1 = "TestSettingsName 1";

        public const string BlogSettingsName2 = "TestSettingsName 2";

        public const string BlogSettingsName3 = "TestSettingsName 3";

        public static IEnumerable<BlogSetting> CreateCollection()
        {
            var setting = new BlogSetting(BlogMetaTestData.BlogKey, BlogSettingsId, BlogSettingsName);
            yield return setting;

            var setting1 = new BlogSetting(BlogMetaTestData.BlogKey1, BlogSettingsId1, BlogSettingsName1);
            yield return setting1;

            var setting2 = new BlogSetting(BlogMetaTestData.BlogKey2, BlogSettingsId2, BlogSettingsName2);
            yield return setting2;

            var setting3 = new BlogSetting(BlogMetaTestData.BlogKey3, BlogSettingsId3, BlogSettingsName3);
            yield return setting3;
        }

        public static BlogSetting Create(string blogKey, string id = null, string name = null)
        {
            id = !string.IsNullOrWhiteSpace(id) ? id : $"{blogKey}Id";
            name = !string.IsNullOrWhiteSpace(name) ? name : $"{blogKey}Name";

            return new BlogSetting(blogKey, id, name);
        }
    }
}
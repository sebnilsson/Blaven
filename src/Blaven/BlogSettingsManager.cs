using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    internal class BlogSettingsManager
    {
        private readonly IReadOnlyList<BlogSetting> blogSettings;

        private readonly Lazy<IReadOnlyList<string>> blogSettingsBlogKeysLazy;

        public BlogSettingsManager(IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSettings == null)
            {
                throw new ArgumentNullException(nameof(blogSettings));
            }

            this.blogSettings = blogSettings.ToList();

            this.blogSettingsBlogKeysLazy =
                new Lazy<IReadOnlyList<string>>(() => this.blogSettings.Select(x => x.BlogKey).ToReadOnlyList());
        }

        public IReadOnlyList<string> BlogKeys => this.blogSettingsBlogKeysLazy.Value;

        public string GetEnsuredBlogKey(string blogKey)
        {
            if (blogKey != null)
            {
                return blogKey;
            }

            string blogSettingsBlogKey = this.blogSettingsBlogKeysLazy.Value.FirstOrDefault();
            if (blogSettingsBlogKey == null)
            {
                string message =
                    $"Param '{nameof(blogKey)}' was null and no default value found in provided '{nameof(this.blogSettings)}'.";
                throw new ArgumentOutOfRangeException(nameof(blogKey), message);
            }

            return blogSettingsBlogKey;
        }

        public ICollection<string> GetEnsuredBlogKeys(IEnumerable<string> blogKeys)
        {
            var blogKeyList = blogKeys?.ToList();

            if (blogKeyList != null && blogKeyList.Any())
            {
                return blogKeyList;
            }

            var blogSettingsBlogKeys = this.blogSettingsBlogKeysLazy.Value.ToList();
            if (!blogSettingsBlogKeys.Any())
            {
                string message =
                    $"Param '{nameof(blogKeys)}' was null and no default values found in provided '{nameof(this.blogSettings)}'.";
                throw new ArgumentOutOfRangeException(nameof(blogKeys), message);
            }

            return blogSettingsBlogKeys;
        }
    }
}
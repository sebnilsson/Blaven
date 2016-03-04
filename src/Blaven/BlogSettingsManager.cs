using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    internal class BlogSettingsManager
    {
        private readonly IReadOnlyList<BlogSetting> blogSettings;

        public BlogSettingsManager(IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSettings == null)
            {
                throw new ArgumentNullException(nameof(blogSettings));
            }

            this.blogSettings = blogSettings.ToReadOnlyList();

            this.BlogKeys =
                this.blogSettings.Where(x => x != null && !string.IsNullOrWhiteSpace(x.BlogKey))
                    .Select(x => x.BlogKey.ToLowerInvariant())
                    .ToReadOnlyList();
        }

        public IReadOnlyList<string> BlogKeys { get; }

        public string GetEnsuredBlogKey(string blogKey)
        {
            if (blogKey != null)
            {
                return blogKey;
            }

            string blogSettingsBlogKey = this.BlogKeys.FirstOrDefault();
            if (blogSettingsBlogKey == null)
            {
                string message =
                    $"Param '{nameof(blogKey)}' was null and no default value found in provided '{nameof(this.blogSettings)}'.";
                throw new ArgumentOutOfRangeException(nameof(blogKey), message);
            }

            return blogSettingsBlogKey.ToLowerInvariant();
        }

        public ICollection<string> GetEnsuredBlogKeys(IEnumerable<string> blogKeys)
        {
            var blogKeyList =
                blogKeys?.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.ToLowerInvariant()).ToList();

            if (blogKeyList != null && blogKeyList.Any())
            {
                return blogKeyList;
            }

            var blogSettingsBlogKeys = this.BlogKeys.ToList();
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
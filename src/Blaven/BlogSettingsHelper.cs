using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class BlogSettingsHelper
    {
        private readonly IReadOnlyList<BlogSetting> blogSettings;

        public BlogSettingsHelper(IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSettings == null)
            {
                throw new ArgumentNullException(nameof(blogSettings));
            }

            this.blogSettings = blogSettings.ToReadOnlyList();

            this.BlogKeys =
                this.blogSettings.Where(x => !string.IsNullOrWhiteSpace(x?.BlogKey))
                    .Select(x => x.BlogKey.ToLowerInvariant())
                    .ToReadOnlyList();
        }

        public IReadOnlyList<string> BlogKeys { get; }

        public BlogSetting GetBlogSetting(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var blogSetting =
                this.blogSettings.FirstOrDefault(x => x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase));
            if (blogSetting == null)
            {
                string message = $"Settings did not contain any item with key '{blogKey}'.";
                throw new KeyNotFoundException(message);
            }

            return blogSetting;
        }

        public string GetEnsuredBlogKey(string blogKey)
        {
            if (blogKey != null)
            {
                return blogKey;
            }

            string blogSettingsBlogKey = this.BlogKeys.FirstOrDefault();
            if (blogSettingsBlogKey == null)
            {
                string message = $"No default item found in {nameof(this.BlogKeys)}.";
                throw new KeyNotFoundException(message);
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
                string message = $"No default items found in {nameof(this.BlogKeys)}.";
                throw new KeyNotFoundException(message);
            }

            return blogSettingsBlogKeys;
        }
    }
}
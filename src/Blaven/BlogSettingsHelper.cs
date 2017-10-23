using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class BlogSettingsHelper
    {
        private readonly IReadOnlyList<BlogSetting> _blogSettings;

        public BlogSettingsHelper(IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSettings == null)
                throw new ArgumentNullException(nameof(blogSettings));

            _blogSettings = blogSettings.ToReadOnlyList();

            BlogKeys = _blogSettings.Select(x => new BlogKey(x.BlogKey)).Where(x => x.HasValue).ToReadOnlyList();
        }

        public IReadOnlyList<BlogKey> BlogKeys { get; }

        public BlogSetting GetBlogSetting(BlogKey blogKey)
        {
            if (blogKey == null)
                throw new ArgumentNullException(nameof(blogKey));
            if (!blogKey.HasValue)
                throw new ArgumentOutOfRangeException(nameof(blogKey), $"{nameof(BlogKey)} must have a value.");

            var blogSetting = _blogSettings.FirstOrDefault(
                x => x.BlogKey.Equals(blogKey.Value, StringComparison.OrdinalIgnoreCase));
            if (blogSetting == null)
            {
                var message = $"Settings did not contain any item with key '{blogKey.Value}'.";
                throw new KeyNotFoundException(message);
            }

            return blogSetting;
        }

        public BlogKey GetEnsuredBlogKey(BlogKey blogKey)
        {
            if (blogKey != null)
                return blogKey;

            string blogSettingsBlogKey = BlogKeys.FirstOrDefault();
            if (blogSettingsBlogKey == null)
            {
                var message = $"No default item found in {nameof(BlogKeys)}.";
                throw new KeyNotFoundException(message);
            }

            return blogSettingsBlogKey.ToLowerInvariant();
        }

        public ICollection<string> GetEnsuredBlogKeys(IEnumerable<string> blogKeys)
        {
            var blogKeyList = blogKeys?.Select(x => new BlogKey(x));

            var ensuredBlogKeys = GetEnsuredBlogKeys(blogKeyList);
            return ensuredBlogKeys;
        }

        public ICollection<string> GetEnsuredBlogKeys(IEnumerable<BlogKey> blogKeys)
        {
            var ensuredBlogKeys = GetEnsuredBlogKeysInternal(blogKeys)
                .Where(x => x.HasValue)
                .Select(x => x.Value)
                .ToList();
            return ensuredBlogKeys;
        }

        private IEnumerable<BlogKey> GetEnsuredBlogKeysInternal(IEnumerable<BlogKey> blogKeys)
        {
            var blogKeyList = blogKeys?.ToList();

            if (blogKeyList != null && blogKeyList.Any())
                return blogKeyList;

            var blogSettingsBlogKeys = BlogKeys;

            if (!blogSettingsBlogKeys.Any())
            {
                var message = $"No default items found in {nameof(BlogKeys)}.";
                throw new KeyNotFoundException(message);
            }

            return blogSettingsBlogKeys;
        }
    }
}
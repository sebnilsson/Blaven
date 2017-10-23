using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Blaven.BlogSources;

namespace Blaven.Configuration
{
    public class AppSettingsConfigService
    {
        private static readonly Regex AppSettingBlogIdRegex = new Regex(
            @"Blaven\.Blogs\.([^\.]+)(?:.*)",
            RegexOptions.IgnoreCase);
        private readonly IDictionary<string, string> _appSettings;

        public AppSettingsConfigService(IDictionary<string, string> appSettings)
        {
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public TBlogSource BuildBlogSource<TBlogSource>(
            Func<string, string, TBlogSource> blogSourceUsernameAndPasswordFactory)
            where TBlogSource : IBlogSource
        {
            if (blogSourceUsernameAndPasswordFactory == null)
                throw new ArgumentNullException(nameof(blogSourceUsernameAndPasswordFactory));

            var username = AppSettingsUtility.GetUsername<TBlogSource>(_appSettings);
            var password = AppSettingsUtility.GetPassword<TBlogSource>(_appSettings);

            var blogSource = blogSourceUsernameAndPasswordFactory(username, password);
            return blogSource;
        }

        public IEnumerable<BlogSetting> GetBlogSettings()
        {
            var blogKeys = from setting in _appSettings
                           where setting.Key != null
                           let match = AppSettingBlogIdRegex.Matches(setting.Key).OfType<Match>().FirstOrDefault()
                           let groupMatch = match?.Groups.OfType<Group>().ElementAtOrDefault(1)
                           let blogKey = groupMatch?.Value
                           where !string.IsNullOrWhiteSpace(blogKey)
                           select blogKey;

            var uniqueBlogKeys = new HashSet<string>(blogKeys, StringComparer.OrdinalIgnoreCase);

            foreach (var blogKey in uniqueBlogKeys)
            {
                var idKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Id");
                var nameKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Name");

                var id = AppSettingsHelper.TryGetValue(idKey, _appSettings);
                var name = AppSettingsHelper.TryGetValue(nameKey, _appSettings);

                yield return new BlogSetting(blogKey, id, name);
            }
        }
    }
}
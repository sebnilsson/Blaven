using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;

using Blaven.BlogSources;

namespace Blaven
{
    public static class AppSettingsFactory
    {
        private static readonly Regex AppSettingBlogIdRegex = new Regex(
            @"Blaven\.Blogs\.([^\.]+)(?:.*)",
            RegexOptions.IgnoreCase);

        public static TBlogSource BuildBlogSource<TBlogSource>(
            Func<string, string, TBlogSource> blogSourceUsernameAndPasswordFactory) where TBlogSource : IBlogSource
        {
            if (blogSourceUsernameAndPasswordFactory == null)
            {
                throw new ArgumentNullException(nameof(blogSourceUsernameAndPasswordFactory));
            }

            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var blogSource = BuildBlogSourceInternal(blogSourceUsernameAndPasswordFactory, appSettings);
            return blogSource;
        }

        public static IEnumerable<BlogSetting> GetBlogSettings()
        {
            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var blogSettings = GetBlogSettingsInternal(appSettings);
            return blogSettings;
        }

        internal static TBlogSource BuildBlogSourceInternal<TBlogSource>(
            Func<string, string, TBlogSource> blogSourceUsernameAndPasswordFactory,
            IDictionary<string, string> appSettings) where TBlogSource : IBlogSource
        {
            if (blogSourceUsernameAndPasswordFactory == null)
            {
                throw new ArgumentNullException(nameof(blogSourceUsernameAndPasswordFactory));
            }
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            string username = BlogSourceAppSettingsUtility.GetUsernameInternal<TBlogSource>(appSettings);
            string password = BlogSourceAppSettingsUtility.GetPasswordInternal<TBlogSource>(appSettings);

            var blogSource = blogSourceUsernameAndPasswordFactory(username, password);
            return blogSource;
        }

        internal static IEnumerable<BlogSetting> GetBlogSettingsInternal(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var blogKeys = from setting in appSettings
                           where setting.Key != null
                           let match = AppSettingBlogIdRegex.Matches(setting.Key).OfType<Match>().FirstOrDefault()
                           let groupMatch = match?.Groups.OfType<Group>().ElementAtOrDefault(1)
                           let blogKey = groupMatch?.Value
                           where !string.IsNullOrWhiteSpace(blogKey)
                           select blogKey;

            var uniqueBlogKeys = new HashSet<string>(blogKeys, StringComparer.InvariantCultureIgnoreCase);

            foreach (var blogKey in uniqueBlogKeys)
            {
                string idKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Id");
                string nameKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Name");

                string id = AppSettingsHelper.TryGetValue(idKey, appSettings);
                string name = AppSettingsHelper.TryGetValue(nameKey, appSettings);

                yield return new BlogSetting(blogKey, id, name);
            }
        }
    }
}
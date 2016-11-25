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
        
        private readonly IDictionary<string, string> appSettings;

        public AppSettingsConfigService(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            this.appSettings = appSettings;
        }

        public IEnumerable<BlogSetting> GetBlogSettings()
        {
            var blogKeys = from setting in this.appSettings
                           where setting.Key != null
                           let match = AppSettingBlogIdRegex.Matches(setting.Key).OfType<Match>().FirstOrDefault()
                           let groupMatch = match?.Groups.OfType<Group>().ElementAtOrDefault(1)
                           let blogKey = groupMatch?.Value
                           where !string.IsNullOrWhiteSpace(blogKey)
                           select blogKey;

            var uniqueBlogKeys = new HashSet<string>(blogKeys, StringComparer.OrdinalIgnoreCase);

            foreach (var blogKey in uniqueBlogKeys)
            {
                string idKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Id");
                string nameKey = string.Format(AppSettingsHelper.BlogsKeyFormat, blogKey, "Name");

                string id = AppSettingsHelper.TryGetValue(idKey, this.appSettings);
                string name = AppSettingsHelper.TryGetValue(nameKey, this.appSettings);

                yield return new BlogSetting(blogKey, id, name);
            }
        }

        public TBlogSource BuildBlogSource<TBlogSource>(
            Func<string, string, TBlogSource> blogSourceUsernameAndPasswordFactory) where TBlogSource : IBlogSource
        {
            if (blogSourceUsernameAndPasswordFactory == null)
            {
                throw new ArgumentNullException(nameof(blogSourceUsernameAndPasswordFactory));
            }

            string username = AppSettingsUtility.GetUsername<TBlogSource>(this.appSettings);
            string password = AppSettingsUtility.GetPassword<TBlogSource>(this.appSettings);

            var blogSource = blogSourceUsernameAndPasswordFactory(username, password);
            return blogSource;
        }
    }
}
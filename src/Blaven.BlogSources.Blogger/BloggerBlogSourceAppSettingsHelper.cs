using System;
using System.Collections.Generic;
using System.Configuration;

namespace Blaven.BlogSources.Blogger
{
    internal static class BloggerBlogSourceAppSettingsHelper
    {
        public const string ApiKeyAppSettingsKey = "Blaven.BlogSources.Blogger.ApiKey";

        public static BloggerBlogSource CreateFromAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var bloggerBlogSource = CreateFromAppSettingsInternal(appSettings);
            return bloggerBlogSource;
        }

        internal static BloggerBlogSource CreateFromAppSettingsInternal(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            string apiKey = GetApiKeyInternal(appSettings);

            var bloggerBlogSource = new BloggerBlogSource(apiKey);
            return bloggerBlogSource;
        }

        internal static string GetApiKeyInternal(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            string apiKey = AppSettingsHelper.GetValue(ApiKeyAppSettingsKey, appSettings);
            return apiKey;
        }
    }
}
using System;
using System.Collections.Generic;
#if NET_45
using System.Configuration;
#endif

namespace Blaven.BlogSources.Blogger
{
    internal static class BloggerBlogSourceAppSettingsHelper
    {
        public const string ApiKeyAppSettingsKey = "Blaven.BlogSources.Blogger.ApiKey";

#if NET_45
        public static BloggerBlogSource CreateFromAppSettings()
        {
            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var bloggerBlogSource = CreateFromAppSettingsInternal(appSettings);
            return bloggerBlogSource;
        }
#endif

        public static BloggerBlogSource CreateFromAppSettingsInternal(IDictionary<string, string> appSettings)
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
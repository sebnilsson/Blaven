using System;
using System.Collections.Generic;
using Blaven.Configuration;

namespace Blaven.BlogSources.Blogger
{
    internal static class BloggerBlogSourceAppSettingsHelper
    {
        public const string ApiKeyAppSettingsKey = "Blaven.BlogSources.Blogger.ApiKey";

        public static BloggerBlogSource CreateFromAppSettingsInternal(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            var apiKey = GetApiKeyInternal(appSettings);

            var bloggerBlogSource = new BloggerBlogSource(apiKey);
            return bloggerBlogSource;
        }

        internal static string GetApiKeyInternal(IDictionary<string, string> appSettings)
        {
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            var apiKey = AppSettingsHelper.GetValue(ApiKeyAppSettingsKey, appSettings);
            return apiKey;
        }
    }
}
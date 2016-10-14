using System;
using System.Collections.Generic;

using Blaven.BlogSources;

namespace Blaven.Configuration
{
    public static class AppSettingsUtility
    {
        public static string GetPassword<TBlogSource>(IDictionary<string, string> appSettings)
            where TBlogSource : IBlogSource
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var config = GetValue<TBlogSource>(AppSettingsHelper.PasswordKey, appSettings, requireValue: false);
            return config;
        }

        public static string GetUsername<TBlogSource>(IDictionary<string, string> appSettings)
            where TBlogSource : IBlogSource
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var config = GetValue<TBlogSource>(AppSettingsHelper.UsernameKey, appSettings, requireValue: false);
            return config;
        }

        public static string GetValue<TBlogSource>(
            string key,
            IDictionary<string, string> appSettings,
            bool requireValue = true) where TBlogSource : IBlogSource
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var type = typeof(TBlogSource);

            string value = GetValue(type, key, appSettings, requireValue);
            return value;
        }

        private static string GetValue(
            Type type,
            string key,
            IDictionary<string, string> appSettings,
            bool requireValue)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            string typeName = type.Name;

            string appSettingsKey = string.Format(AppSettingsHelper.BlogSourcesKeyFormat, typeName, key);

            string value = requireValue
                               ? AppSettingsHelper.GetValue(appSettingsKey, appSettings)
                               : AppSettingsHelper.TryGetValue(appSettingsKey, appSettings);
            return value;
        }
    }
}
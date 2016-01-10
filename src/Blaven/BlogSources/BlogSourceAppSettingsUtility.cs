using System;
using System.Collections.Generic;
using System.Configuration;

namespace Blaven.BlogSources
{
    public static class BlogSourceAppSettingsUtility
    {
        public static string GetPassword<TBlogSource>() where TBlogSource : IBlogSource
        {
            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var config = GetPasswordInternal<TBlogSource>(appSettings);
            return config;
        }

        public static string GetUsername<TBlogSource>() where TBlogSource : IBlogSource
        {
            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var config = GetUsernameInternal<TBlogSource>(appSettings);
            return config;
        }

        public static string GetValue<TBlogSource>(string key) where TBlogSource : IBlogSource
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var appSettings = ConfigurationManager.AppSettings.ToDictionaryIgnoreCase();

            var config = GetValueInternal<TBlogSource>(key, appSettings);
            return config;
        }

        internal static string GetPasswordInternal<TBlogSource>(IDictionary<string, string> appSettings)
            where TBlogSource : IBlogSource
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var config = GetValueInternal<TBlogSource>(AppSettingsHelper.PasswordKey, appSettings);
            return config;
        }

        internal static string GetUsernameInternal<TBlogSource>(IDictionary<string, string> appSettings)
            where TBlogSource : IBlogSource
        {
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            var config = GetValueInternal<TBlogSource>(AppSettingsHelper.UsernameKey, appSettings);
            return config;
        }

        internal static string GetValueInternal<TBlogSource>(string key, IDictionary<string, string> appSettings)
            where TBlogSource : IBlogSource
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

            string value = GetValue(type, key, appSettings);
            return value;
        }

        private static string GetValue(Type type, string key, IDictionary<string, string> appSettings)
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

            string value = AppSettingsHelper.GetValue(appSettingsKey, appSettings);
            return value;
        }
    }
}
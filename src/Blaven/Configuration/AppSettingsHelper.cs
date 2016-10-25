using System;
using System.Collections.Generic;

namespace Blaven.Configuration
{
    public static class AppSettingsHelper
    {
        public const string BlogsKeyFormat = "Blaven.Blogs.{0}.{1}";

        public const string BlogSourcesKeyFormat = "Blaven.BlogSources.{0}.{1}";

        public const string PasswordKey = "Password";

        public const string UsernameKey = "Username";

        public static string GetValue(string appSettingsKey, IDictionary<string, string> appSettings)
        {
            if (appSettingsKey == null)
            {
                throw new ArgumentNullException(nameof(appSettingsKey));
            }
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            if (!appSettings.ContainsKey(appSettingsKey))
            {
                string message = $"AppSettings does not contain key '{appSettingsKey}'.";
                throw new KeyNotFoundException(message);
            }

            string value = appSettings[appSettingsKey];
            return value;
        }

        public static string TryGetValue(string appSettingsKey, IDictionary<string, string> appSettings)
        {
            if (appSettingsKey == null)
            {
                throw new ArgumentNullException(nameof(appSettingsKey));
            }
            if (appSettings == null)
            {
                throw new ArgumentNullException(nameof(appSettings));
            }

            if (!appSettings.ContainsKey(appSettingsKey))
            {
                return null;
            }

            string value = GetValue(appSettingsKey, appSettings);
            return value;
        }
    }
}
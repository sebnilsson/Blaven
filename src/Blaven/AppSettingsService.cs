using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

using Raven.Abstractions.Data;

namespace Blaven
{
    /// <summary>
    /// A static service-class to handle the application's settings.
    /// </summary>
    public static class AppSettingsService
    {
        private const string DefaultBloggerSettingsPath = "~/BlogSettings.json";

        private static readonly Lazy<string> BloggerSettingsPathLazy = new Lazy<string>(
            () =>
                {
                    string value = GetConfigValue(
                        "Blaven.BloggerSettingsPath", defaultValue: DefaultBloggerSettingsPath);

                    string resolvedPath;
                    try
                    {
                        resolvedPath = HttpContext.Current.Server.MapPath(value);
                    }
                    catch (Exception)
                    {
                        resolvedPath = DefaultBloggerSettingsPath;
                    }

                    return resolvedPath;
                });

        /// <summary>
        /// Gets the Blogger-settings file-path. Uses config-key "Blaven.BloggerSettingsPath". App-relative paths gets resolved. Defaults to "~/BloggerSettings.json".
        /// </summary>
        public static string BloggerSettingsPath
        {
            get
            {
                return BloggerSettingsPathLazy.Value;
            }
        }

        private static readonly Lazy<int> CacheTimeLazy =
            new Lazy<int>(() => GetConfigValueInt("Blaven.CacheTime", defaultValue: 5));

        /// <summary>
        /// Gets the default cache-time, in minutes. Uses config-key "Blaven.CacheTime". Defaults to 10 (minutes).
        /// </summary>
        public static int CacheTime
        {
            get
            {
                return CacheTimeLazy.Value;
            }
        }

        private static readonly Lazy<IEnumerable<string>> ExcludeTransformersLazy = new Lazy<IEnumerable<string>>(
            () =>
                {
                    string configValue = GetConfigValue("Blaven.ExcludeTransformers");

                    var excludeTransformers =
                        configValue.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    return excludeTransformers;
                });

        /// <summary>
        /// Gets or sets the Transformers to exclude.
        /// Uses config-key "Blaven.ExcludeTransformers".
        /// </summary>
        public static IEnumerable<string> ExcludeTransformers
        {
            get
            {
                return ExcludeTransformersLazy.Value;
            }
        }

        private static readonly Lazy<bool> EnsureBlogsRefreshedLazy =
            new Lazy<bool>(() => GetConfigValueBool("Blaven.EnsureBlogsRefreshed", defaultValue: true));

        /// <summary>
        /// Gets or sets if the BlogService should automatically ensure that blogs are refresh upon instantiation. Defaults to true.
        /// Uses config-key "Blaven.EnsureBlogsRefreshed".
        /// </summary>
        public static bool EnsureBlogsRefreshed
        {
            get
            {
                return EnsureBlogsRefreshedLazy.Value;
            }
        }

        private static readonly Lazy<int> PageSizeLazy =
            new Lazy<int>(() => GetConfigValueInt("Blaven.PageSize", defaultValue: 5));

        /// <summary>
        /// Gets the default page-size. Uses "Blaven.PageSize". Defaults to 5.
        /// </summary>
        public static int PageSize
        {
            get
            {
                return PageSizeLazy.Value;
            }
        }

        private static readonly Lazy<ConnectionStringParser<RavenConnectionStringOptions>> ConnectionStringParserLazy =
            new Lazy<ConnectionStringParser<RavenConnectionStringOptions>>(
                () =>
                    {
                        string urlKey = GetConfigValue("Blaven.RavenDbStoreUrlKey", throwException: true);
                        string urlValue = GetConfigValue(urlKey);

                        var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(urlValue);
                        parser.Parse();
                        return parser;
                    });

        private static string ravenDbStoreUrl;

        /// <summary>
        /// Gets the URL to the RavenDB store.
        /// Uses config-key "Blaven.RavenDbStoreUrlKey".
        /// </summary>
        public static string RavenDbStoreUrl
        {
            get
            {
                return ravenDbStoreUrl
                       ?? (ravenDbStoreUrl = ConnectionStringParserLazy.Value.ConnectionStringOptions.Url);
            }
        }

        private static string ravenDbStoreApiKey;

        /// <summary>
        /// Gets the API-key used for the RavenDB store.
        /// Uses config-key "Blaven.RavenDbStoreUrlKey".
        /// </summary>
        public static string RavenDbStoreApiKey
        {
            get
            {
                return ravenDbStoreApiKey
                       ?? (ravenDbStoreApiKey = ConnectionStringParserLazy.Value.ConnectionStringOptions.ApiKey);
            }
        }

        private static readonly Lazy<bool> RefreshAsyncLazy =
            new Lazy<bool>(() => GetConfigValueBool("Blaven.RefreshAsync", defaultValue: true));

        /// <summary>
        /// Gets if the refresh of blogs should by done async. Defaults to true.
        /// Uses config-key "Blaven.RefreshAsync".
        /// </summary>
        public static bool RefreshAsync
        {
            get
            {
                return RefreshAsyncLazy.Value;
            }
        }

        internal static string GetConfigValue(string configKey, bool throwException = false, string defaultValue = "")
        {
            string value = ConfigurationManager.AppSettings[configKey];
            if (throwException && string.IsNullOrWhiteSpace(value))
            {
                throw new ConfigurationErrorsException(
                    string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }

            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }

        internal static bool GetConfigValueBool(
            string configKey, bool throwException = false, bool defaultValue = default(bool))
        {
            string value = GetConfigValue(configKey, throwException);
            bool result;
            if (!bool.TryParse(value, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        internal static int GetConfigValueInt(
            string configKey, bool throwException = false, int defaultValue = default(int))
        {
            string value = GetConfigValue(configKey, throwException);
            int result;
            if (!int.TryParse(value, out result))
            {
                result = defaultValue;
            }

            return result;
        }
    }
}
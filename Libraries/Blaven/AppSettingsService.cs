using System;
using System.Configuration;
using System.Web;

using Raven.Abstractions.Data;

namespace Blaven {
    /// <summary>
    /// A static service-class to handle the application's settings.
    /// </summary>
    public static class AppSettingsService {
        private static Lazy<string> _bloggerSettingsPath = new Lazy<string>(() => {
            string value = GetConfigValue("Blaven.BloggerSettingsPath");
            value = (!string.IsNullOrWhiteSpace(value)) ? value : "~/BloggerSettings.json";

            string resolvedPath = string.Empty;
            try {
                resolvedPath = HttpContext.Current.Server.MapPath(value);
            }
            catch(Exception) { }

            return resolvedPath;
        });
        /// <summary>
        /// Gets the Blogger-settings file-path. Uses config-key "Blaven.BloggerSettingsPath". App-relative paths gets resolved. Defaults to "~/BloggerSettings.json".
        /// </summary>
        public static string BloggerSettingsPath {
            get {
                return _bloggerSettingsPath.Value;
            }
        }

        private static Lazy<int> _cacheTime = new Lazy<int>(() => {
            string value = GetConfigValue("Blaven.CacheTime");
            int result = 0;
            if(!int.TryParse(value, out result)) {
                result = 5;
            }
            return result;
        });
        /// <summary>
        /// Gets the default cache-time. Uses config-key "Blaven.CacheTime". Defaults to 5.
        /// </summary>
        public static int CacheTime {
            get {
                return _cacheTime.Value;
            }
        }

        private static Lazy<bool> _ensureBlogsRefreshed = new Lazy<bool>(() => {
            string configValue = GetConfigValue("Blaven.EnsureBlogsRefreshed");

            bool result = true;
            if(!bool.TryParse(configValue, out result)) {
                result = true;
            }

            return result;
        });
        /// <summary>
        /// Gets or sets if the BlogService should automatically ensure that blogs are refresh upon instantiation. Defaults to true.
        /// Uses config-key "Blaven.EnsureBlogsRefreshed".
        /// </summary>
        public static bool EnsureBlogsRefreshed {
            get {
                return _ensureBlogsRefreshed.Value;
            }
        }

        private static Lazy<bool> _ignoreBloggerServiceFailure = new Lazy<bool>(() => {
            string configValue = GetConfigValue("Blaven.IgnoreBloggerServiceFailure");

            bool result = true;
            if(!bool.TryParse(configValue, out result)) {
                result = true;
            }

            return result;
        });
        /// <summary>
        /// Gets or sets if the BlogService should ignore a failed call to the Blogger-service. Defaults to true.
        /// Uses config-key "Blaven.IgnoreBloggerServiceFailure".
        /// </summary>
        public static bool IgnoreBloggerServiceFailure {
            get {
                return _ignoreBloggerServiceFailure.Value;
            }
        }

        private static Lazy<int> _pageSize = new Lazy<int>(() => {
            string value = GetConfigValue("Blaven.PageSize");
            int result = 0;
            if(!int.TryParse(value, out result)) {
                result = 5;
            }
            return result;
        });
        /// <summary>
        /// Gets the default page-size. Uses "Blaven.PageSize". Defaults to 5.
        /// </summary>
        public static int PageSize {
            get {
                return _pageSize.Value;
            }
        }

        private static Lazy<ConnectionStringParser<RavenConnectionStringOptions>> _connectionStringParser =
            new Lazy<ConnectionStringParser<RavenConnectionStringOptions>>(() => {
                string key = GetConfigValue("Blaven.RavenDbStoreUrlKey", throwException: true);
                string value = GetConfigValue(key);

                var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(value);
                parser.Parse();
                return parser;
        });

        private static string _ravenDbStoreUrl;
        /// <summary>
        /// Gets the URL to the RavenDB store.
        /// Uses config-key "Blaven.RavenDbStoreUrlKey".
        /// </summary>
        public static string RavenDbStoreUrl {
            get {
                if(_ravenDbStoreUrl == null) {
                    _ravenDbStoreUrl = _connectionStringParser.Value.ConnectionStringOptions.Url ?? string.Empty;
                }
                return _ravenDbStoreUrl;
            }
        }

        private static string _ravenDbStoreApiKey;
        /// <summary>
        /// Gets the API-key used for the RavenDB store.
        /// Uses config-key "Blaven.RavenDbStoreUrlKey".
        /// </summary>
        public static string RavenDbStoreApiKey {
            get {
                if(_ravenDbStoreApiKey == null) {
                    _ravenDbStoreApiKey = _connectionStringParser.Value.ConnectionStringOptions.ApiKey ?? string.Empty;
                }
                return _ravenDbStoreApiKey;
            }
        }

        private static Lazy<bool> _refreshAsync = new Lazy<bool>(() => {
            string configValue = GetConfigValue("Blaven.RefreshAsync");

            bool result = true;
            if(!bool.TryParse(configValue, out result)) {
                result = true;
            }

            return result;
        });
        /// <summary>
        /// Gets if the refresh of blogs should by done async. Defaults to true.
        /// Uses config-key "Blaven.RefreshAsync".
        /// </summary>
        public static bool RefreshAsync {
            get {
                return _refreshAsync.Value;
            }
        }

        internal static string GetConfigValue(string configKey, bool throwException = false) {
            string value = ConfigurationManager.AppSettings[configKey];
            if(throwException && string.IsNullOrWhiteSpace(value)) {
                throw new ConfigurationErrorsException(string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }
            return value ?? string.Empty;
        }
    }
}

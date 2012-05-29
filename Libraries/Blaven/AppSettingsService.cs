using System;
using System.Configuration;

using Raven.Abstractions.Data;

namespace Blaven {
    /// <summary>
    /// A static service-class to handle the application's settings.
    /// </summary>
    public static class AppSettingsService {
        private static Lazy<string> _bloggerSettingsPath = new Lazy<string>(() => {
            string value = GetConfigValue("Blaven.BloggerSettingsPath", throwException: false);
            return (!string.IsNullOrWhiteSpace(value)) ? value : "~/BloggerSettings.json";
        });
        /// <summary>
        /// Gets the Blogger-settings file-path. Uses config-key "Blaven.BloggerSettingsPath". Defaults to "~/BloggerSettings.json".
        /// </summary>
        public static string BloggerSettingsPath {
            get {
                return _bloggerSettingsPath.Value;
            }
        }

        private static Lazy<int> _cacheTime = new Lazy<int>(() => {
            string value = GetConfigValue("Blaven.CacheTime", throwException: false);
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

        private static Lazy<int> _pageSize = new Lazy<int>(() => {
            string value = GetConfigValue("Blaven.PageSize", throwException: false);
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
                string key = GetConfigValue("Blaven.RavenDbStoreUrlKey");
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

        private static Lazy<BlogRefreshMode> _refreshMode = new Lazy<BlogRefreshMode>(() => {
            string configValue = GetConfigValue("Blaven.RefreshMode", throwException: false);
            
            var result = BlogRefreshMode.Asynchronously;
            if(!Enum.TryParse<BlogRefreshMode>(configValue, out result)) {
                switch(configValue.ToLowerInvariant()) {
                    case "background":
                        result = BlogRefreshMode.BackgroundService;
                        break;
                    case "sync":
                        result = BlogRefreshMode.Synchronously;
                        break;
                    default:
                        result = BlogRefreshMode.Asynchronously;
                        break;
                }
            }
            return result;
        });
        /// <summary>
        /// Gets which mode should be used for refreshing data from blogs. Defaults to BlogRefreshMode.Asynchronously.
        /// Uses config-key "Blaven.RefreshMode". Short-hand config-values supported: "background", "sync" and "async".
        /// </summary>
        public static BlogRefreshMode RefreshMode {
            get {
                return _refreshMode.Value;
            }
        }

        internal static string GetConfigValue(string configKey, bool throwException = true) {
            string value = ConfigurationManager.AppSettings[configKey];
            if(throwException && string.IsNullOrWhiteSpace(value)) {
                throw new ConfigurationErrorsException(string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }
            return value ?? string.Empty;
        }
    }
}

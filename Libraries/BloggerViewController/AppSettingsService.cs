using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BloggerViewController {
    /// <summary>
    /// A static service-class to handle the application's settings.
    /// </summary>
    public static class AppSettingsService {
        private static string _bloggerSettingsPath;
        /// <summary>
        /// Gets the Blogger-settings file-path. Looks at the config-key "BloggerViewController.BloggerSettingsPath". Defaults to "~/BloggerSettings.json".
        /// </summary>
        public static string BloggerSettingsPath {
            get {
                if(_bloggerSettingsPath == null) {
                    string value = GetConfigValue("BloggerViewController.BloggerSettingsPath", throwException: false);
                    _bloggerSettingsPath = (!string.IsNullOrWhiteSpace(value)) ? value : "~/BloggerSettings.json";
                }
                return _bloggerSettingsPath;
            }
        }

        private static int? _cacheTime;
        /// <summary>
        /// Gets the default cache-time. Looks at the config-key "BloggerViewController.CacheTime". Defaults to 5.
        /// </summary>
        public static int CacheTime {
            get {
                if(!_cacheTime.HasValue) {
                    string value = GetConfigValue("BloggerViewController.CacheTime", throwException: false);
                    int result = 0;
                    if(!int.TryParse(value, out result)) {
                        result = 5;
                    }
                    _cacheTime = result;
                }
                return _cacheTime.Value;
            }
        }

        private static int? _pageSize;
        /// <summary>
        /// Gets the default page-size. Looks at the config-key "BloggerViewController.PageSize". Defaults to 5.
        /// </summary>
        public static int PageSize {
            get {
                if(!_pageSize.HasValue) {
                    string value = GetConfigValue("BloggerViewController.PageSize", throwException: false);
                    int result = 0;
                    if(!int.TryParse(value, out result)) {
                        result = 5;
                    }
                    _pageSize = result;
                }
                return _pageSize.Value;
            }
        }

        private static string _ravenDbStoreUrl;
        /// <summary>
        /// Gets the URL to the RavenDB store from a configuration-key.
        /// Looks at the config-key "BloggerViewController.RavenDbStoreUrlKey" to find correct config-key to get the value from.
        /// </summary>
        public static string RavenDbStoreUrl {
            get {
                if(_ravenDbStoreUrl == null) {
                    string key = GetConfigValue("BloggerViewController.RavenDbStoreUrlKey");
                    string value = GetConfigValue(key);
                    _ravenDbStoreUrl = value ?? string.Empty;
                }
                return _ravenDbStoreUrl;
            }
        }

        private static bool? _useBackgroundService;
        /// <summary>
        /// Gets if the application should use a background-service to update its blog-data.
        /// Looks at the config-key "BloggerViewController.UseBackgroundService". Defaults to false.
        /// </summary>
        public static bool UseBackgroundService {
            get {
                if(!_useBackgroundService.HasValue) {
                    string configValue = GetConfigValue("BloggerViewController.UseBackgroundService", throwException: false);
                    bool result;
                    bool.TryParse(configValue, out result);
                    _useBackgroundService = result;
                }
                return _useBackgroundService.Value;
            }
        }

        internal static string GetConfigValue(string configKey, bool throwException = true) {
            string value = ConfigurationManager.AppSettings[configKey];
            if(throwException && string.IsNullOrWhiteSpace(value)) {
                throw new ConfigurationErrorsException(string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }
            return value;
        }
    }
}

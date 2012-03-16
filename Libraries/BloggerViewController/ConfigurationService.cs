using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BloggerViewController {
    public static class ConfigurationService {
        /*public static BlogConfiguration DefaultConfiguration { get { return GetConfiguration(string.Empty); } }
        public static BlogConfiguration GetConfiguration(string blogKey) {
            return new BlogConfiguration(blogKey);
        }*/

        /*private static IEnumerable<BloggerSetting> _bloggerSettings;
        public static IEnumerable<BloggerSetting> BloggerSettings {
            get {
                if(_bloggerSettings == null) {
                    
                }
                return _bloggerSettings;
            }
        }*/

        private static string _bloggerSettingsPath;
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

        private static bool? _useBackgroundService;
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

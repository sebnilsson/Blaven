using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BloggerViewController {
    public static class ConfigurationService {
        public static BlogConfiguration DefaultConfiguration { get { return GetConfiguration(string.Empty); } }
        public static BlogConfiguration GetConfiguration(string blogKey) {
            return new BlogConfiguration(blogKey);
        }

        private static IEnumerable<string> _blogList;
        public static IEnumerable<string> BlogList {
            get {
                if(_blogList == null) {
                    string value = GetConfigValue("BloggerViewController.BlogList", throwException: false);
                    _blogList = value.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
                }
                return _blogList;
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

        internal static string GetConfigValue(string configKey, string blogKey = null, bool throwException = true) {
            if(!string.IsNullOrWhiteSpace(blogKey)) {
                configKey = string.Format("{0}.{1}", blogKey, configKey);
            }

            string value = ConfigurationManager.AppSettings[configKey];
            if(throwException && string.IsNullOrWhiteSpace(value)) {
                throw new ConfigurationErrorsException(string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }
            return value;
        }
    }
}

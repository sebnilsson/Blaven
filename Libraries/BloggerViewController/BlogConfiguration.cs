using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BloggerViewController {
    public static class BlogConfiguration {
        private static Lazy<string> _blogId = new Lazy<string>(() => {
            return GetConfigValue("BloggerViewController.BlogId");
        });
        public static string BlogId {
            get {
                return _blogId.Value;
            }
        }

        private static Lazy<string> _username = new Lazy<string>(() => {
            return GetConfigValue("BloggerViewController.Username");
        });
        public static string Username {
            get {
                return _username.Value;
            }
        }

        private static Lazy<string> _password = new Lazy<string>(() => {
            return GetConfigValue("BloggerViewController.Password");
        });
        public static string Password {
            get {
                return _password.Value;
            }
        }

        private static Lazy<int> _pageSize = new Lazy<int>(() => {
            string value = GetConfigValue("BloggerViewController.PageSize", false);
            int result = 0;
            if(!int.TryParse(value, out result)) {
                result = 5;
            }

            return result;
        });
        public static int PageSize {
            get {
                return _pageSize.Value;
            }
        }

        private static Lazy<int> _cacheTime = new Lazy<int>(() => {
            string value = GetConfigValue("BloggerViewController.CacheTime", false);
            int result = 0;
            if(!int.TryParse(value, out result)) {
                result = 5;
            }

            return result;
        });
        public static int CacheTime {
            get {
                return _cacheTime.Value;
            }
        }

        private static string GetConfigValue(string configKey, bool throwException = true) {
            string value = ConfigurationManager.AppSettings[configKey];
            if(throwException && string.IsNullOrWhiteSpace(value)) {
                throw new ConfigurationErrorsException(string.Format("Could not find configuration-value for key '{0}'.", configKey));
            }
            return value;
        }
    }
}

using System.Configuration;

namespace BloggerViewController {
    public class BlogConfiguration {
        private string _blogKey;
        public BlogConfiguration(string blogKey) {
            _blogKey = blogKey;
        }

        private string _blogId;
        public string BlogId {
            get {
                if(string.IsNullOrWhiteSpace(_blogId)) {
                    _blogId = GetConfigValue("BloggerViewController.BlogId", _blogKey);
                }
                return _blogId;
            }
        }

        private string _username;
        public string Username {
            get {
                if(string.IsNullOrWhiteSpace(_username)) {
                    _username = GetConfigValue("BloggerViewController.Username", _blogKey);
                }
                return _username;
            }
        }

        private string _password;
        public string Password {
            get {
                if(string.IsNullOrWhiteSpace(_password)) {
                    _password = GetConfigValue("BloggerViewController.Password", _blogKey);
                }
                return _password;
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

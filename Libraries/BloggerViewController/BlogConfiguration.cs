namespace BloggerViewController {
    public class BlogConfiguration {
        private string _blogKey;
        public BlogConfiguration(string blogKey) {
            _blogKey = blogKey ?? string.Empty;
        }

        public string BlogKey {
            get { return _blogKey; }
        }

        private string _blogId;
        public string BlogId {
            get {
                if(string.IsNullOrWhiteSpace(_blogId)) {
                    _blogId = ConfigurationService.GetConfigValue("BloggerViewController.BlogId", _blogKey);
                }
                return _blogId;
            }
        }

        private string _password;
        public string Password {
            get {
                if(string.IsNullOrWhiteSpace(_password)) {
                    _password = ConfigurationService.GetConfigValue("BloggerViewController.Password", _blogKey);
                }
                return _password;
            }
        }

        private string _username;
        public string Username {
            get {
                if(string.IsNullOrWhiteSpace(_username)) {
                    _username = ConfigurationService.GetConfigValue("BloggerViewController.Username", _blogKey);
                }
                return _username;
            }
        }
    }
}

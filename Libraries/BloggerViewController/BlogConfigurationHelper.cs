namespace BloggerViewController {
    public static class BlogConfigurationHelper {
        public static BlogConfiguration DefaultConfiguration { get { return Get(string.Empty); } }
        public static BlogConfiguration Get(string blogKey) {
            return new BlogConfiguration(blogKey);
        }
        
        private static int? _pageSize;
        public static int PageSize {
            get {
                if(!_pageSize.HasValue) {
                    string value = BlogConfiguration.GetConfigValue("BloggerViewController.PageSize", throwException: false);
                    int result = 0;
                    if(!int.TryParse(value, out result)) {
                        result = 5;
                    }
                    _pageSize = result;
                }
                return _pageSize.Value;
            }
        }

        private static int? _cacheTime;
        public static int CacheTime {
            get {
                if(!_cacheTime.HasValue) {
                    string value = BlogConfiguration.GetConfigValue("BloggerViewController.CacheTime", throwException: false);
                    int result = 0;
                    if(!int.TryParse(value, out result)) {
                        result = 5;
                    }
                    _cacheTime = result;
                }
                return _cacheTime.Value;
            }
        }

        private static IBlogStore _blogStore;
        public static IBlogStore BlogStore {
            get {
                if(_blogStore == null) {
                    string value = BlogConfiguration.GetConfigValue("BloggerViewController.BlogStore", throwException: false);
                    switch(value.ToLowerInvariant()) {
                        case "disk":
                            _blogStore = new DiskBlogStore();
                            break;
                        default:
                            _blogStore = new MemoryBlogStore();
                            break;
                    }
                }
                return _blogStore;
            }
        }
    }
}

using System;

namespace BloggerViewController {
    public class BlogService {
        private BlogConfiguration _configuration;
        private BloggerHelper _bloggerHelper;
        private IBlogStore _store;

        public BlogService(BlogConfiguration configuration = null, IBlogStore store = null) {
            _configuration = configuration ?? BlogConfigurationHelper.DefaultConfiguration;
            _store = store ?? BlogConfigurationHelper.BlogStore;

            _bloggerHelper = new BloggerHelper(_configuration);
        }

        public BlogInfo GetInfo() {
            EnsureStoreIsUpdated();

            var info = _store.GetBlogInfo();
            return info;
        }

        public BlogSelection GetSelection(int pageIndex, int? pageSize = null) {
            EnsureStoreIsUpdated();

            var selection = _store.GetBlogSelection(pageIndex, pageSize);
            return selection;
        }

        public BlogPost GetPost(string blogId) {
            EnsureStoreIsUpdated();

            var post = _store.GetBlogPost(blogId);
            return post;
        }

        public void InitStore() {
            BlogStoreCacheHandler.EnsureStoreIsUpdated(_bloggerHelper, _store, BlogConfigurationHelper.CacheTime, false);
        }
        
        internal void EnsureStoreIsUpdated() {
            // Check if cache is updated
            BlogStoreCacheHandler.EnsureStoreIsUpdated(_bloggerHelper, _store, BlogConfigurationHelper.CacheTime);
        }
    }
}

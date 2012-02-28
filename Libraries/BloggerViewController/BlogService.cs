using System;

namespace BloggerViewController {
    public class BlogService {
        private string _blogId;
        private string _password;
        private string _username;
        private BloggerHelper _bloggerHelper;
        private IBlogStore _store;

        public BlogService(string blogId = null, string username = null, string password = null, IBlogStore store = null) {
            _blogId = blogId ?? BlogConfiguration.BlogId;
            _username = username ?? BlogConfiguration.Username;
            _password = password ?? BlogConfiguration.Password;
            _store = store ?? new MemoryBlogStore();

            _bloggerHelper = new BloggerHelper(_blogId, _username, _password);
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

        private void EnsureStoreIsUpdated() {
            // Check if cache is updated
            bool isCacheUpToDate = (_store.LastUpdate.HasValue && _store.LastUpdate.Value.AddMinutes(BlogConfiguration.CacheTime) > DateTime.Now);
            if(isCacheUpToDate) {
                return;
            }

            // If not, updated with BloggerHelper and call IBlogStore.Update
            var bloggerDocument = _bloggerHelper.GetBloggerDocument(_store.LastUpdate);
            _store.Update(bloggerDocument);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Data;

namespace BloggerViewController {
    public class BlogService {
        private IEnumerable<BloggerSetting> _settings;
        private IBlogStore _store;
        private BloggerHelper _bloggerHelper;

        public BlogService(IBlogStore store, params BloggerSetting[] settings) {
            if(store == null) {
                throw new ArgumentNullException("store");
            }

            if(settings == null) {
                throw new ArgumentNullException("settings");
            }

            _settings = settings.AsEnumerable();
            _bloggerHelper = new BloggerHelper(_settings);
            _store = store;
        }

        public BlogInfo GetInfo(string blogKey = null) {
            blogKey = blogKey ?? string.Empty;
            EnsureBlogIsUpdated(blogKey);

            var info = _store.GetBlogInfo(blogKey);
            return info;
        }

        public BlogSelection GetSelection(int pageIndex, int? pageSize = null, string blogKey = null, Func<BlogPost, bool> predicate = null) {
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The page-index must be a positive number.");
            }

            blogKey = blogKey ?? string.Empty;
            EnsureBlogIsUpdated(blogKey);

            var actualPageSize = pageSize.GetValueOrDefault(ConfigurationService.PageSize);

            var selection = _store.GetBlogSelection(pageIndex, actualPageSize, blogKey, predicate);
            return selection;
        }

        public BlogPost GetPost(string permaLink, string blogKey = null) {
            blogKey = blogKey ?? string.Empty;
            EnsureBlogIsUpdated(blogKey);

            var post = _store.GetBlogPost(permaLink, blogKey);
            return post;
        }

        public void UpdateBlog(string blogKey = null) {
            blogKey = blogKey ?? string.Empty;

            var bloggerDocument = _bloggerHelper.GetBloggerDocument(blogKey);
            _store.Update(bloggerDocument, blogKey);
        }

        private static Dictionary<string, object> _locks = new Dictionary<string, object>();
        private static object GetLock(string blogKey) {
            object objLock;
            if(!_locks.TryGetValue(blogKey, out objLock)) {
                objLock = _locks[blogKey] = new object();
            }
            return objLock;
        }

        private void EnsureBlogIsUpdated(string blogKey) {
            if(ConfigurationService.UseBackgroundService) {
                return;                
            }

            if(_store.GetIsBlogUpdated(blogKey)) {
                return;
            }

            var objLock = GetLock(blogKey);
            lock(objLock) {
                if(_store.GetIsBlogUpdated(blogKey)) {
                    return;
                }
                
                UpdateBlog(blogKey);
            }
        }
    }
}

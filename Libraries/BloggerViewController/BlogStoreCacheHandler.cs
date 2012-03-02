using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloggerViewController {
    internal class BlogStoreCacheHandler {
        private int _cacheTime;
        private IBlogStore _store;
        private BloggerHelper _bloggerHelper;

        public BlogStoreCacheHandler(BloggerHelper bloggerHelper, IBlogStore store, int? cacheTime = null) {
            _bloggerHelper = bloggerHelper;
            _cacheTime = cacheTime.GetValueOrDefault(BlogConfiguration.CacheTime);
            _store = store;
        }

        public bool IsCacheUpToDate {
            get {
                return (_store.LastUpdate.HasValue && _store.LastUpdate.Value.AddMinutes(_cacheTime) > DateTime.Now);
            }
        }

        public void UpdateStore() {
            var bloggerDocument = _bloggerHelper.GetBloggerDocument(_store.LastUpdate);
            _store.Update(bloggerDocument);

            //// If not, updated with BloggerHelper and call IBlogStore.Update
            //try {
            //    _isBloggerDocumentUpdating = true;
            //    lock(_updateLock) {
            //        // If the store has any data then perform update async
            //        if(_store.HasAnyData) {
            //            var callback = new System.Threading.WaitCallback((obj) => {
            //                var bloggerDocument = _bloggerHelper.GetBloggerDocument(_store.LastUpdate);
            //                _store.Update(bloggerDocument);
            //                _isBloggerDocumentUpdating = false;
            //            });
            //            System.Threading.ThreadPool.QueueUserWorkItem(callback, null);
            //        } else {
            //            var bloggerDocument = _bloggerHelper.GetBloggerDocument(_store.LastUpdate);
            //            _store.Update(bloggerDocument);
            //            _isBloggerDocumentUpdating = false;
            //        }
            //    }
            //}
            //catch(Exception) {
            //    _isBloggerDocumentUpdating = false;
            //    // Silent fail
            //}
        }
    }
}

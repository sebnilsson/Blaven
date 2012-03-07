using System;

namespace BloggerViewController {
    internal class BlogStoreCacheHandler {
        private static bool GetIsCacheUpToDate(IBlogStore store, int cacheTime) {
            return (store.LastUpdate.HasValue && store.LastUpdate.Value.AddMinutes(cacheTime) > DateTime.Now);
        }

        private static bool _isUpdating = false;

        private static readonly object _updateLock = new object();

        public static void EnsureStoreIsUpdated(BloggerHelper bloggerHelper, IBlogStore store, int? cacheTime = null, bool alwaysThrowOnError = true) {
            var cacheTimeHours = cacheTime.GetValueOrDefault(BlogConfigurationHelper.CacheTime);

            if(_isUpdating) {
                return;
            }

            lock(_updateLock) {
                if(_isUpdating || GetIsCacheUpToDate(store, cacheTimeHours)) {
                    return;
                }

                _isUpdating = true;

                Action<object> action = (obj) => {
                    try {
                        var blogStore = obj as IBlogStore;

                        var bloggerDocument = bloggerHelper.GetBloggerDocument(blogStore.LastUpdate);
                        blogStore.Update(bloggerDocument);
                    }
                    catch(Exception) {
                        if(alwaysThrowOnError) {
                            throw;
                        }

                        // If the fetch fails, but there is data in store, don't throw exception
                        if(!store.HasData) {
                            throw;
                        }
                    }
                    finally {
                        _isUpdating = false;
                    }
                };

                if(!store.HasData) {
                    action(store);
                } else {
                    var callback = new System.Threading.WaitCallback(action);
                    System.Threading.ThreadPool.QueueUserWorkItem(callback, store);
                }

            }
        }
    }
}

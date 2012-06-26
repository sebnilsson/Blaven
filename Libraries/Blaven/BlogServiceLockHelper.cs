using System.Collections.Generic;

namespace Blaven {
    internal class BlogServiceLockHelper {
        public static bool GetIsBlogRefreshing(string blogKey) {
            if(!_refreshingLockStore.ContainsKey(blogKey)) {
                return false;
            }
            return _refreshingLockStore[blogKey];
        }

        public static void SetIsBlogRefreshing(string blogKey, bool setLocked = true) {
            _refreshingLockStore[blogKey] = setLocked;
        }

        private static Dictionary<string, bool> _refreshingLockStore = new Dictionary<string, bool>();
        
        public static object GetRefreshedLock(string key) {
            if(!_refreshedLockStore.ContainsKey(key)) {
                lock(_refreshedLockStoreLock) {
                    if(!_refreshedLockStore.ContainsKey(key)) {
                        _refreshedLockStore[key] = new object();
                    }
                }
            }
            return _refreshedLockStore[key];
        }
        private static readonly object _refreshedLockStoreLock = new object();
        private static Dictionary<string, object> _refreshedLockStore = new Dictionary<string, object>();
    }
}

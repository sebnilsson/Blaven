using System;
using System.Collections.Generic;

namespace Blaven {
    [Serializable]
    public class BlogsRefreshException : BlavenException {
        public BlogsRefreshException(IEnumerable<string> refreshBlogKeys, IEnumerable<string> failedWaitBlogKeys)
            : base("There was an error when waiting for blogs to refresh the first time.") {
            RefreshBlogKeys = refreshBlogKeys;
            FailedWaitBlogKeys = failedWaitBlogKeys;
        }

        public IEnumerable<string> RefreshBlogKeys { get; private set; }
        public IEnumerable<string> FailedWaitBlogKeys { get; private set; }
    }
}

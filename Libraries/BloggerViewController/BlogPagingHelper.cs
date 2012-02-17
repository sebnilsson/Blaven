using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogPagingHelper {
        public BlogPagingHelper(IEnumerable<BlogPost> allPosts, IEnumerable<BlogPostDetail> currentPosts) {
            if(!allPosts.Any() || !currentPosts.Any()) {
                return;
            }

            HasNextItems = (allPosts.LastOrDefault().ID != currentPosts.LastOrDefault().ID);
            HasPreviousItems = (allPosts.FirstOrDefault().ID != currentPosts.FirstOrDefault().ID);
        }

        public bool HasNextItems { get; private set; }
        public bool HasPreviousItems { get; private set; }
    }
}

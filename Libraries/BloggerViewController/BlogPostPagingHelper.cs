using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BloggerViewController {
    public class BlogPostPagingHelper {
        public BlogPostPagingHelper(IEnumerable<BlogPost> allPosts, BlogPost currentPost) {
            NextPost = allPosts.SkipWhile(p => p != currentPost).Skip(1).FirstOrDefault();
            PreviousPost = allPosts.SkipWhile(p => p != currentPost).Skip(-1).FirstOrDefault();
        }

        public BlogPost NextPost { get; set; }
        public BlogPost PreviousPost { get; set; }
    }
}

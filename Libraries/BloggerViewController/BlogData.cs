using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogData {
        public BlogData(BlogInfo info = null, IEnumerable<BlogPost> posts = null) {
            this.Info = info;
            this.Posts = posts ?? Enumerable.Empty<BlogPost>();
        }

        public BlogInfo Info { get; set; }

        public IEnumerable<BlogPost> Posts { get; set; }
    }
}

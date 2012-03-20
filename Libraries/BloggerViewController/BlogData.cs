using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogData {
        /// <summary>
        /// An object holding all the information about a blog from a parsed XML-document from BLogger.
        /// </summary>
        public BlogData() {
            this.Posts = Enumerable.Empty<BlogPost>();
        }
        
        /// <summary>
        /// The information about the parsed blog.
        /// </summary>
        public BlogInfo Info { get; set; }

        /// <summary>
        /// The posts from the parsed blog.
        /// </summary>
        public IEnumerable<BlogPost> Posts { get; set; }
    }
}

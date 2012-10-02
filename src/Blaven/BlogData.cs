using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven {
    internal class BlogData {
        /// <summary>
        /// An object holding all the information about a blog from a parsed XML-document from Blogger.
        /// </summary>
        public BlogData() {
            this.Info = new BlogInfo();
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

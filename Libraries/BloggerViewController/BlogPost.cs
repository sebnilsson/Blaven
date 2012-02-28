using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogPost {
        public BlogPost(IEnumerable<string> categories = null) {
            this.Categories = categories ?? Enumerable.Empty<string>();
        }

        public IEnumerable<string> Categories { get; set; }
        public string Content { get; set; }
        public string EditUri { get; set; }
        public string ID { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string Uri { get; set; }
    }
}

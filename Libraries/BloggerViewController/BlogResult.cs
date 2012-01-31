using System;
using System.Collections.Generic;

namespace BloggerViewController {
    public class BlogResult {
        public IEnumerable<string> Categories { get; set; }
        public string Description { get; set; }
        public IEnumerable<BlogPost> Posts { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogPost {
        public BlogPost(IEnumerable<string> labels = null) {
            this.Labels = labels ?? Enumerable.Empty<string>();
        }

        public string Content { get; set; }

        public string ID { get; set; }

        public IEnumerable<string> Labels { get; set; }

        public DateTime Published { get; set; }

        public string Title { get; set; }

        public DateTime Updated { get; set; }

        public string PermaLinkAbsolute { get; set; }

        public string PermaLinkRelative { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Google.GData.Client;

namespace BloggerViewController {
    public class BlogPost {
        public BlogPost() {

        }

        internal BlogPost(AtomEntry entry) {
            ID = entry.Id.Uri.Content;
            Published = entry.Published;
            Title = entry.Title.Text;
            Uri = entry.Id.Uri.Content;
        }

        public string ID { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }
        public string Uri { get; set; }
    }
}

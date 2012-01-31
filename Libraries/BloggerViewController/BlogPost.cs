using System;
using System.Collections.Generic;
using System.Linq;

using Google.GData.Client;

namespace BloggerViewController {
    public class BlogPost {
        public BlogPost() {

        }

        internal BlogPost(AtomEntry entry) {
            Categories = (entry.Categories != null)
                ? entry.Categories.Select(cat => cat.Term) : Enumerable.Empty<string>();
            EditUri = (entry.EditUri != null) ? entry.EditUri.Content : null;
            Published = entry.Published;
            Summary = entry.Summary.Text;
            Text = entry.Content.Content;
            Title = entry.Title.Text;
            Updated = entry.Updated;
            Uri = entry.Id.Uri.Content;
        }

        public IEnumerable<string> Categories { get; set; }
        public string EditUri { get; set; }
        public DateTime Published { get; set; }
        public string Summary { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
        public string Uri { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using Google.GData.Client;

namespace BloggerViewController {
    public class BlogPostDetail : BlogPost {
        public BlogPostDetail() {

        }

        internal BlogPostDetail(AtomEntry entry) {
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
        public string Summary { get; set; }
        public string Text { get; set; }
        public DateTime Updated { get; set; }
    }
}

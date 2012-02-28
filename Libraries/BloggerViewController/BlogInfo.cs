using System;
using System.Collections.Generic;

namespace BloggerViewController {
    public class BlogInfo {
        public BlogInfo(IDictionary<string, int> categories = null, IDictionary<DateTime, int> postDates = null) {
            Categories = categories ?? new Dictionary<string, int>();
            PostDates = postDates ?? new Dictionary<DateTime, int>();
        }

        public IDictionary<string, int> Categories { get; set; }
        public IDictionary<DateTime, int> PostDates { get; set; }
        public string Subtitle { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
    }
}

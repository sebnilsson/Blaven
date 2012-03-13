﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogInfo {
        public BlogInfo(Dictionary<string, int> labels = null, IEnumerable<string> friendlyPermaLinks = null, Dictionary<DateTime, int> postDates = null) {
            Labels = labels ?? new Dictionary<string, int>();
            FriendlyPermaLinks = friendlyPermaLinks ?? Enumerable.Empty<string>();
            PostDates = postDates ?? new Dictionary<DateTime, int>();
        }

        public string BlogKey { get; set; }

        public Dictionary<string, int> Labels { get; set; }

        public IEnumerable<string> FriendlyPermaLinks { get; set; }

        public Dictionary<DateTime, int> PostDates { get; set; }

        public string Subtitle { get; set; }

        public string Title { get; set; }

        public DateTime Updated { get; set; }
    }
}

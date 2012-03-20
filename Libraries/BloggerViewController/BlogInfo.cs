using System;
using System.Collections.Generic;

namespace BloggerViewController {
    /// <summary>
    /// Represents the info/meta-data of a blog.
    /// </summary>
    public class BlogInfo {
        public BlogInfo() {
            this.Labels = new Dictionary<string, int>();
            this.PostDates = new Dictionary<DateTime, int>();
        }

        /// <summary>
        /// The unique key to identify the blog.
        /// </summary>
        public string BlogKey { get; set; }

        /// <summary>
        /// The labels/tags that the blog's posts contains, inclouding the count of labels.
        /// </summary>
        public Dictionary<string, int> Labels { get; set; }

        /// <summary>
        /// A collection of blog's posts' dates, grouped by year and months.
        /// </summary>
        public Dictionary<DateTime, int> PostDates { get; set; }

        /// <summary>
        /// The alternative sub-title for the blog.
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// The main title for the blog.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date of last update of the blog.
        /// </summary>
        public DateTime Updated { get; set; }
    }
}

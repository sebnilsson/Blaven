using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggerViewController {
    [DataContract]
    public class BlogInfo {
        public BlogInfo(Dictionary<string, int> categories = null, IEnumerable<string> friendlyPermaLinks = null, Dictionary<DateTime, int> postDates = null) {
            Categories = categories ?? new Dictionary<string, int>();
            FriendlyPermaLinks = friendlyPermaLinks ?? Enumerable.Empty<string>();
            PostDates = postDates ?? new Dictionary<DateTime, int>();
        }

        [DataMember]
        public Dictionary<string, int> Categories { get; set; }
        [DataMember]
        public Dictionary<DateTime, int> PostDates { get; set; }
        [DataMember]
        public IEnumerable<string> FriendlyPermaLinks { get; set; }
        [DataMember]
        public string Subtitle { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public DateTime Updated { get; set; }
    }
}

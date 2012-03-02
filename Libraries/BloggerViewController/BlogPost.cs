using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggerViewController {
    [DataContract]
    public class BlogPost {
        public BlogPost(IEnumerable<string> categories = null) {
            this.Categories = categories ?? Enumerable.Empty<string>();
        }

        [DataMember]
        public IEnumerable<string> Categories { get; set; }
        [DataMember]
        public string Content { get; set; }
        [DataMember]
        public string FriendlyPermaLink { get; set; }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public DateTime Published { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public DateTime Updated { get; set; }
    }
}

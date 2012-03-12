using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BloggerViewController {
    [DataContract]
    public class BlogData {
        public BlogData(BlogInfo info = null, IEnumerable<BlogPost> posts = null) {
            this.Info = info;
            this.Posts = posts ?? Enumerable.Empty<BlogPost>();
        }

        [DataMember]
        public BlogInfo Info { get; set; }
        [DataMember]
        public IEnumerable<BlogPost> Posts { get; set; }
    }
}

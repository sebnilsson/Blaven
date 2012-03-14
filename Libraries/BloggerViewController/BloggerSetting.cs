using System.Runtime.Serialization;

namespace BloggerViewController {
    [DataContract]
    public class BloggerSetting {
        [DataMember(Name = "blogKey")]
        public string BlogKey { get ; set; }

        [DataMember(Name = "blogId")]
        public string BlogId { get ; set; }

        [DataMember(Name = "password")]
        public string Password { get ; set; }

        [DataMember(Name = "username")]
        public string Username { get; set; }
    }
}

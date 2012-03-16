using System.Runtime.Serialization;

namespace BloggerViewController {
    [DataContract]
    public class BloggerSetting {
        [DataMember(Name = "blogKey")]
        public string BlogKey { get ; set; }

        [DataMember(Name = "blogId")]
        public string BlogId { get ; set; }

        [DataMember(Name = "passwordKey")]
        public string PasswordKey { get ; set; }

        [DataMember(Name = "usernameKey")]
        public string UsernameKey { get; set; }

        public string Password { get; set; }

        public string Username { get; set; }
    }
}

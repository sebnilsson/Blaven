using System.Runtime.Serialization;

namespace Blaven.Blogger {
    /// <summary>
    /// Represents a Blogger-setting.
    /// </summary>
    [DataContract]
    public class BloggerSetting {
        /// <summary>
        /// The base-URL for the blog. Leave empty to use default value from Blogger.
        /// </summary>
        [DataMember(Name = "baseUrl")]
        public string BaseUrl { get; set; }

        /// <summary>
        /// The unique identifier for the blog.
        /// </summary>
        [DataMember(Name = "blogKey")]
        public string BlogKey { get ; set; }

        /// <summary>
        /// The ID of the blog on Blogger.
        /// </summary>
        [DataMember(Name = "blogId")]
        public string BlogId { get ; set; }
        
        [DataMember(Name = "passwordKey")]
        internal string PasswordKey { get ; set; }

        [DataMember(Name = "usernameKey")]
        internal string UsernameKey { get; set; }

        /// <summary>
        /// The password for the account on Blogger.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The username for the account on Blogger.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The URI to the Blogger-document. Will be resolved to correct URI from BlogId, if left empty.
        /// </summary>
        public string BloggerUri { get; set; }
    }
}

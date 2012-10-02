using System;
using System.Runtime.Serialization;

namespace Blaven {
    [Serializable]
    public class BlavenBlogException : BlavenException {
        public BlavenBlogException(string blogKey) {
            this.BlogKey = blogKey;
        }
        public BlavenBlogException(string blogKey, string message) : base(message) {
            this.BlogKey = blogKey;
        }
        public BlavenBlogException(string blogKey, string message, Exception inner) : base(message, inner) {
            this.BlogKey = blogKey;
        }
        protected BlavenBlogException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string BlogKey { get; private set; }
    }
}

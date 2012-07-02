using System;
using System.Xml.Linq;

namespace Blaven.Blogger {
    [Serializable]
    public class BloggerParsingException : BlavenBlogException {
        public BloggerParsingException(string blogKey, XDocument bloggerDocument, Exception inner)
            : base(blogKey, "There was an error when parsing the Blogger-document", inner) {
            this.BloggerDocument = bloggerDocument;
        }

        public XDocument BloggerDocument { get; private set; }
    }
}

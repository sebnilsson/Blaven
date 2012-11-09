using System;

namespace Blaven.Blogger
{
    [Serializable]
    public class BloggerParsingException : BlavenBlogException
    {
        public BloggerParsingException(string blogKey, string bloggerDocumentContent, Exception inner)
            : base(blogKey, "There was an error when parsing the Blogger-document", inner)
        {
            this.BloggerDocumentContent = bloggerDocumentContent;
        }

        public string BloggerDocumentContent { get; private set; }
    }
}
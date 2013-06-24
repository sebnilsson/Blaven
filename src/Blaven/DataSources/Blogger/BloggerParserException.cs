using System;

namespace Blaven.DataSources.Blogger
{
    public class BloggerParserException : BlavenBlogException
    {
        public BloggerParserException(string blogKey, string bloggerDocumentContent, Exception inner)
            : base(blogKey, inner, "There was an error when parsing the Blogger-document")
        {
            this.BloggerDocumentContent = bloggerDocumentContent;
        }

        public string BloggerDocumentContent { get; private set; }
    }
}
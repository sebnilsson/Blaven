using System;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerBlogSourceException : BlavenException
    {
        public BloggerBlogSourceException(string message)
            : base(message)
        {
        }

        public BloggerBlogSourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
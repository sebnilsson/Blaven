using System;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiRequestExecuteException : BlavenException
    {
        public BloggerApiRequestExecuteException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
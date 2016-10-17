using System;

namespace Blaven.Synchronization
{
    public class BlogSyncBlogSourceException : BlogSyncException
    {
        public BlogSyncBlogSourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
using System;

namespace Blaven.Synchronization
{
    public class BlogSyncException : BlavenException
    {
        public BlogSyncException(string message)
            : base(message)
        {
        }

        public BlogSyncException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
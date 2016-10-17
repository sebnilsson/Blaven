using System;

namespace Blaven.Synchronization
{
    public class BlogSyncDataStorageException : BlogSyncException
    {
        public BlogSyncDataStorageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
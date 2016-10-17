namespace Blaven.Synchronization
{
    public class BlogSyncDataStorageResultException : BlogSyncException
    {
        public BlogSyncDataStorageResultException(string message)
            : base(message)
        {
        }
    }
}
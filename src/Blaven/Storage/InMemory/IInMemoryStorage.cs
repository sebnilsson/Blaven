namespace Blaven.Storage.InMemory
{
    public interface IInMemoryStorage : IStorageQueryData
    {
        void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta);

        void CreateOrUpdatePost(BlogKey blogKey, BlogPost post);

        void RemovePosts(BlogKey blogKey, string? id = null);
    }
}

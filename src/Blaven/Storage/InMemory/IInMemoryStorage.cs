using System.Linq;

namespace Blaven.Storage.InMemory
{
    public interface IInMemoryStorage
    {
        IQueryable<BlogMeta> Metas { get; }

        IQueryable<BlogPost> Posts { get; }

        void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta);

        void CreateOrUpdatePost(BlogKey blogKey, BlogPost post);

        void RemovePosts(BlogKey blogKey, string? id = null);
    }
}

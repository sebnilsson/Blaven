using System.Linq;

namespace Blaven.Storage
{
    public interface IStorageQueryData
    {
        IQueryable<BlogMeta> Metas { get; }

        IQueryable<BlogPost> Posts { get; }
    }
}

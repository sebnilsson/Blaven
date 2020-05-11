using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public interface IStorageSyncRepository
    {
        Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter);

        Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter);

        Task Update(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter,
            BlogMeta? meta,
            IEnumerable<BlogPost> insertedPosts,
            IEnumerable<BlogPost> updatedPosts,
            IEnumerable<BlogPostBase> deletedPosts);
    }
}

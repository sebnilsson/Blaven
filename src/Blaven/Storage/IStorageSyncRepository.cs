using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public interface IStorageSyncRepository
    {
        Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt);

        Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt);

        Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            IEnumerable<BlogPost> insertedPosts,
            IEnumerable<BlogPost> updatedPosts,
            IEnumerable<BlogPostBase> deletedPosts,
            DateTimeOffset? lastUpdatedAt);
    }
}

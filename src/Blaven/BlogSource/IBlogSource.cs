using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSource
{
    public interface IBlogSource
    {
        Task<IReadOnlyList<BlogPost>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt = null);

        Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt = null);
    }
}

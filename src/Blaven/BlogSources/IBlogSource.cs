using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        Task<IReadOnlyList<BlogPost>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null);

        Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null);
    }
}

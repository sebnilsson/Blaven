using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        Task<IReadOnlyList<BlogPost>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt);
    }
}
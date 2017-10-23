using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        Task<IReadOnlyList<BlogPost>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt);
    }
}
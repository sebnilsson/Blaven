using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.BlogSources;

namespace Blaven.Data
{
    public interface IDataStorage
    {
        Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting);

        Task<IReadOnlyList<BlogPostBase>> GetPostBases(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta);

        Task SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet);
    }
}
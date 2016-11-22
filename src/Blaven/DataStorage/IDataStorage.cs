using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.Synchronization;

namespace Blaven.DataStorage
{
    public interface IDataStorage
    {
        Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting);

        Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta);

        Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet);
    }
}
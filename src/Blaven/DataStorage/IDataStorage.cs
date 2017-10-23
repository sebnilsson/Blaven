using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaven.Synchronization;

namespace Blaven.DataStorage
{
    public interface IDataStorage
    {
        Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting);

        Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta);

        Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet);
    }
}
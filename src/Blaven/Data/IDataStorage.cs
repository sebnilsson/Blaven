using System;
using System.Collections.Generic;

using Blaven.BlogSources;

namespace Blaven.Data
{
    public interface IDataStorage
    {
        DateTime? GetLastPostUpdatedAt(BlogSetting blogSetting);

        IReadOnlyCollection<BlogPostBase> GetPostBases(BlogSetting blogSetting);

        void SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta);

        void SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet);
    }
}
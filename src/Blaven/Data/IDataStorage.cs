using System.Collections.Generic;

using Blaven.BlogSources;

namespace Blaven.Data
{
    public interface IDataStorage
    {
        IEnumerable<BlogPostBase> GetBlogPosts(BlogSetting blogSetting);

        void SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta);

        void SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet);
    }
}
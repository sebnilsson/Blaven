using System.Collections.Generic;

using Blaven.BlogSources;

namespace Blaven.Data
{
    public interface IDataStorage
    {
        IEnumerable<BlogPostBase> GetBlogPosts(string blogKey);

        void SaveBlogMeta(string blogKey, BlogMeta blogMeta);

        void SaveChanges(string blogKey, BlogSourceChangeSet changeSet);
    }
}
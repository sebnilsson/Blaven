using System.Collections.Generic;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        BlogMeta GetMeta(string blogKey);

        BlogSourceChangeSet GetChanges(string blogKey, IEnumerable<BlogPostBase> dbBlogPosts);
    }
}
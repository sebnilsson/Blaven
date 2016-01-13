using System.Collections.Generic;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        BlogMeta GetMeta(BlogSetting blogSetting);

        BlogSourceChangeSet GetChanges(BlogSetting blogSetting, IEnumerable<BlogPostBase> dbPosts);
    }
}
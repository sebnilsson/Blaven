using System;
using System.Collections.Generic;

namespace Blaven.BlogSources
{
    public interface IBlogSource
    {
        BlogMeta GetMeta(BlogSetting blogSetting, DateTime lastUpdatedAt);

        BlogSourceChangeSet GetChanges(
            BlogSetting blogSetting,
            DateTime lastUpdatedAt,
            IEnumerable<BlogPostBase> existingPosts);
    }
}
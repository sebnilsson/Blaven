using System.Collections.Generic;

namespace Blaven.BlogSources
{
    public class BlogSourceData
    {
        public BlogSourceData(
            BlogKey blogKey,
            BlogMeta? meta,
            IReadOnlyList<BlogPost> posts)
        {
            BlogKey = blogKey;
            Meta = meta;
            Posts = posts;
        }

        public BlogKey BlogKey { get; }

        public BlogMeta? Meta { get; }

        public IReadOnlyList<BlogPost> Posts { get; }
    }
}

using System.Collections.Generic;

namespace Blaven.DataSources
{
    public class DataSourceRefreshResult
    {
        public BlogInfo BlogInfo { get; set; }

        public IEnumerable<BlogPost> ModifiedBlogPosts { get; set; }

        public IEnumerable<string> RemovedBlogPostIds { get; set; }
    }
}
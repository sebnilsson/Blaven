using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Blaven.DataSources;

namespace Blaven.Test.DataSources
{
    public class MockDataSource : IDataSource
    {
        private readonly int sleepMs;

        private readonly BlogInfo blogInfo;

        private readonly IEnumerable<BlogPost> modifiedBlogPosts;

        private readonly IEnumerable<string> removedBlogPostIds;

        public MockDataSource(
            int sleepMs = 2000,
            BlogInfo blogInfo = null,
            IEnumerable<BlogPost> modifiedBlogPosts = null,
            IEnumerable<string> removedBlogPostIds = null)
        {
            this.sleepMs = sleepMs;
            this.blogInfo = blogInfo ?? new BlogInfo();
            this.modifiedBlogPosts = modifiedBlogPosts ?? Enumerable.Empty<BlogPost>();
            this.removedBlogPostIds = removedBlogPostIds ?? Enumerable.Empty<string>();
        }

        public DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        {
            Thread.Sleep(this.sleepMs);

            return new DataSourceRefreshResult
                       {
                           BlogInfo = this.blogInfo,
                           ModifiedBlogPosts = this.modifiedBlogPosts,
                           RemovedBlogPostIds = this.removedBlogPostIds
                       };
        }
    }
}
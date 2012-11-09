using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class BlogPostsOverviewByKey : AbstractIndexCreationTask<BlogPost, BlogPostsOverviewByKey.ReduceResult>
    {
        public BlogPostsOverviewByKey()
        {
            this.Map =
                posts =>
                from post in posts where !post.IsDeleted select new { post.BlogKey, ID = post.Id, post.Updated, };
        }

        public class ReduceResult
        {
            public string BlogKey { get; set; }

            public string ID { get; set; }

            public DateTime Updated { get; set; }
        }
    }
}
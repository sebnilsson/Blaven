using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes {
    public class BlogPostsOverviewByKey : AbstractIndexCreationTask<BlogPost, BlogPostsOverviewByKey.ReduceResult> {
        public class ReduceResult {
            public string BlogKey { get; set; }
            public string ID { get; set; }
            public DateTime Updated { get; set; }
        }

        public BlogPostsOverviewByKey() {
            Map = posts => from post in posts
                           where !post.IsDeleted
                           select new {
                               BlogKey = post.BlogKey,
                               ID = post.Id,
                               Updated = post.Updated,
                           };
        }
    }
}

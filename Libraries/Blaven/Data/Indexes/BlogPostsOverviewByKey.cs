using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.Indexes {
    public class BlogPostsOverviewByKey : AbstractIndexCreationTask<BlogPost, BlogPostsOverviewByKey.ReduceResult> {
        public class ReduceResult {
            public string BlogKey { get; set; }
            public string ID { get; set; }
            public DateTime Updated { get; set; }
        }

        public BlogPostsOverviewByKey() {
            Map = posts => from post in posts
                           select new {
                               BlogKey = post.BlogKey,
                               ID = post.ID,
                               Updated = post.Updated,
                           };
        }
    }
}

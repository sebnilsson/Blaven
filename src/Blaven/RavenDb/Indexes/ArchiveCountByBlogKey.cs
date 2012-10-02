using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes {
    public class ArchiveCountByBlogKey : AbstractIndexCreationTask<BlogPost, ArchiveCountByBlogKey.ReduceResult> {
        public class ReduceResult {
            internal string BlogKey { get; set; }
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }

        public ArchiveCountByBlogKey() {
            Map = posts => from post in posts
                           where !post.IsDeleted
                           let date = new DateTime(post.Published.Year, post.Published.Month, 1)
                           select new {
                               BlogKey = post.BlogKey,
                               Date = date,
                               Count = 1,
                           };

            Reduce = results => from result in results
                                group result by new {
                                    Date = result.Date,
                                    BlogKey = result.BlogKey,
                                } into g
                                select new {
                                    BlogKey = g.Key.BlogKey,
                                    Date = g.Key.Date,
                                    Count = g.Sum(x => x.Count),
                                };
        }
    }
}

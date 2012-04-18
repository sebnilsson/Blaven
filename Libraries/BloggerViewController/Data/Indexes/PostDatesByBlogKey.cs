using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Client.Indexes;

namespace BloggerViewController.Data.Indexes {
    public class PostDatesByBlogKey : AbstractIndexCreationTask<BlogPost, PostDatesByBlogKey.ReduceResult> {
        public class ReduceResult {
            internal string BlogKey { get; set; }
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }

        public PostDatesByBlogKey() {
            Map = posts => from post in posts
                           let date = new DateTime(post.Published.Year, post.Published.Month, 1)
                           select new {
                               BlogKey = post.BlogKey,
                               Date = date,
                               Count = 1,
                           };

            Reduce = results => from result in results
                                group result by new { Date = result.Date, BlogKey = result.BlogKey, }
                                    into g
                                    select new {
                                        BlogKey = g.Key.BlogKey,
                                        Date = g.Key.Date,
                                        Count = g.Sum(x => x.Count),
                                    };
        }
    }
}

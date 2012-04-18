using System.Collections.Generic;
using System.Linq;

using Raven.Client.Indexes;

namespace BloggerViewController.Data.Indexes {
    public class LabelsCountByBlogKey : AbstractIndexCreationTask<BlogPost, LabelsCountByBlogKey.ReduceResult> {
        public class ReduceResult {
            internal string BlogKey { get; set; }
            public string Label { get; set; }
            public int Count { get; set; }
        }

        public LabelsCountByBlogKey() {
            Map = posts => from post in posts
                           from label in post.Labels
                           select new {
                               BlogKey = post.BlogKey,
                               Label = label,
                               Count = 1,
                           };

            Reduce = results => from result in results
                                group result by new { Label = result.Label, BlogKey = result.BlogKey, }
                                    into g
                                    select new {
                                        BlogKey = g.Key.BlogKey,
                                        Label = g.Key.Label,
                                        Count = g.Sum(x => x.Count),
                                    };
        }
    }
}

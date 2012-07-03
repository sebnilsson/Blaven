using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes {
    public class TagsCountByBlogKey : AbstractIndexCreationTask<BlogPost, TagsCountByBlogKey.ReduceResult> {
        public class ReduceResult {
            public string BlogKey { get; set; }
            public string Tag { get; set; }
            public int Count { get; set; }
        }

        public TagsCountByBlogKey() {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new {
                               BlogKey = post.BlogKey,
                               Tag = tag,
                               Count = 1,
                           };

            Reduce = results => from result in results
                                group result by new {
                                    result.BlogKey,
                                    result.Tag
                                } into g
                                select new {
                                    BlogKey = g.Key.BlogKey,
                                    Tag = g.Key.Tag,
                                    Count = g.Sum(x => x.Count),
                                };
            
        }
    }
}
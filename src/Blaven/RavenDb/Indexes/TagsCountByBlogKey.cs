using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class TagsCountByBlogKey : AbstractIndexCreationTask<BlogPost, TagsCountByBlogKey.ReduceResult>
    {
        public TagsCountByBlogKey()
        {
            this.Map =
                posts =>
                from post in posts
                from tag in post.Tags
                where !post.IsDeleted
                select new { post.BlogKey, Tag = tag, Count = 1, };

            this.Reduce = results => from result in results
                                     group result by new { result.BlogKey, result.Tag }
                                     into g select new { g.Key.BlogKey, g.Key.Tag, Count = g.Sum(x => x.Count), };

        }

        public class ReduceResult
        {
            public string BlogKey { get; set; }

            public string Tag { get; set; }

            public int Count { get; set; }
        }
    }
}
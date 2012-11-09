using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class ArchiveCountByBlogKey : AbstractIndexCreationTask<BlogPost, ArchiveCountByBlogKey.ReduceResult>
    {
        public ArchiveCountByBlogKey()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted
                                let date = new DateTime(post.Published.Year, post.Published.Month, 1)
                                select new { post.BlogKey, Date = date, Count = 1, };

            this.Reduce = results => from result in results
                                     group result by new { result.Date, result.BlogKey, }
                                     into g select new { g.Key.BlogKey, g.Key.Date, Count = g.Sum(x => x.Count), };
        }

        public class ReduceResult
        {
            internal string BlogKey { get; set; }

            public DateTime Date { get; set; }

            public int Count { get; set; }
        }
    }
}
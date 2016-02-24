using System;
using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class ArchiveCountIndex : AbstractIndexCreationTask<BlogPost, BlogArchiveItem>
    {
        public ArchiveCountIndex()
        {
            this.Map = posts => from post in posts
                                where post.PublishedAt > DateTime.MinValue
                                let date = new DateTime(post.PublishedAt.Value.Year, post.PublishedAt.Value.Month, 1)
                                select new BlogArchiveItem { BlogKey = post.BlogKey, Date = date, Count = 1, };

            this.Reduce = results => from result in results
                                     group result by new { result.BlogKey, result.Date }
                                     into g
                                     orderby g.Key.Date descending
                                     select
                                         new BlogArchiveItem
                                             {
                                                 BlogKey = g.Key.BlogKey,
                                                 Date = g.Key.Date,
                                                 Count = g.Sum(x => x.Count)
                                             };

            this.Index(x => x.BlogKey, FieldIndexing.Default);
        }
    }
}
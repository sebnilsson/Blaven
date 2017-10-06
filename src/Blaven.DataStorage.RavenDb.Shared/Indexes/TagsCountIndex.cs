using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.DataStorage.RavenDb.Indexes
{
    public class TagsCountIndex : AbstractIndexCreationTask<BlogPost, BlogTagItem>
    {
        public TagsCountIndex()
        {
            this.Map = posts => from post in posts
                                from tag in post.BlogPostTags
                                where post.PublishedAt > DateTime.MinValue
                                select new BlogTagItem { BlogKey = post.BlogKey, Name = tag.Text, Count = 1, };

            this.Reduce = results => from result in results
                                     group result by new { result.BlogKey, Name = result.Name.ToLowerInvariant() }
                                     into g
                                     select
                                         new BlogTagItem
                                             {
                                                 BlogKey = g.Key.BlogKey.ToLowerInvariant(),
                                                 Name = g.Select(x => x.Name).FirstOrDefault(),
                                                 Count = g.Sum(x => x.Count),
                                             };
        }
    }
}
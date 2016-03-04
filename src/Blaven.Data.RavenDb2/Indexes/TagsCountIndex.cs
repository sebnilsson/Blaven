﻿using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class TagsCountIndex : AbstractIndexCreationTask<BlogPost, BlogTagItem>
    {
        public TagsCountIndex()
        {
            this.Map = posts => from post in posts
                                from tagName in post.Tags
                                where post.PublishedAt > DateTime.MinValue
                                select new BlogTagItem { BlogKey = post.BlogKey, Name = tagName, Count = 1, };

            this.Reduce = results => from result in results
                                     group result by new { result.BlogKey, Name = result.Name.ToLowerInvariant() }
                                     into g
                                     orderby g.Key.Name ascending
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
using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Blaven.DataStorage.RavenDb.Indexes
{
    public class BlogPostsIndex : AbstractIndexCreationTask<BlogPost, BlogPost>
    {
        public BlogPostsIndex()
        {
            Map = posts => from post in posts
                           where post.PublishedAt > DateTime.MinValue
                           select new BlogPost
                                  {
                                      BlogKey = post.BlogKey,
                                      SourceId = post.SourceId,
                                      PublishedAt = post.PublishedAt,
                                      BlogPostTags = post.BlogPostTags,
                                      UpdatedAt = post.UpdatedAt
                                  };

            Index(x => x.SourceId, FieldIndexing.NotAnalyzed);
        }
    }
}
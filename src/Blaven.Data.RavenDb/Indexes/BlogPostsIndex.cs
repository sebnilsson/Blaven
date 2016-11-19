using System;
using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb.Indexes
{
    public class BlogPostsIndex : AbstractIndexCreationTask<BlogPost, BlogPost>
    {
        public BlogPostsIndex()
        {
            this.Map = posts => from post in posts
                                where post.PublishedAt > DateTime.MinValue
                                select
                                new BlogPost
                                    {
                                        BlogKey = post.BlogKey,
                                        SourceId = post.SourceId,
                                        PublishedAt = post.PublishedAt,
                                        BlogPostTags = post.BlogPostTags,
                                        UpdatedAt = post.UpdatedAt
                                    };
            
            this.Index(x => x.SourceId, FieldIndexing.NotAnalyzed);
        }
    }
}
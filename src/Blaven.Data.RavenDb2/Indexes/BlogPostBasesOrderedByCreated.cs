using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class BlogPostBasesOrderedByCreated : AbstractIndexCreationTask<BlogPost, BlogPostBase>
    {
        public BlogPostBasesOrderedByCreated()
        {
            this.Map = posts => from post in posts
                                where post.PublishedAt > DateTime.MinValue
                                orderby post.PublishedAt descending
                                select
                                    new BlogPostBase
                                        {
                                            BlavenId = post.BlavenId,
                                            BlogKey = post.BlogKey,
                                            Hash = post.Hash,
                                            SourceId = post.SourceId
                                        };
        }
    }
}
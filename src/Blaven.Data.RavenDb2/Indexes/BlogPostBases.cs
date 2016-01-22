using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class BlogPostBases : AbstractIndexCreationTask<BlogPost, BlogPostBase>
    {
        public BlogPostBases()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted && post.PublishedAt > DateTime.MinValue
                                select
                                    new
                                        {
                                            post.Id,
                                            post.BlavenId,
                                            post.BlogKey,
                                            post.Published,
                                            post.Tags,
                                            post.Title,
                                            post.Updated,
                                            post.UrlSlug,
                                        };
        }
    }
}
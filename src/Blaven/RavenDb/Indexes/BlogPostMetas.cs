using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class BlogPostMetas : AbstractIndexCreationTask<BlogPost, BlogPostMeta>
    {
        public BlogPostMetas()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted
                                select
                                    new
                                        {
                                            post.Id,
                                            post.BlogKey,
                                            post.DataSourceId,
                                            post.Checksum,
                                            post.Published
                                        };
        }
    }
}
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class BlogPostHeads : AbstractIndexCreationTask<BlogPost, BlogPostHead>
    {
        public BlogPostHeads()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted
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
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class BlogPostsSimple : AbstractIndexCreationTask<BlogPost, BlogPostSimple>
    {
        public BlogPostsSimple()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted
                                select
                                    new
                                        {
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
using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class BlogPostsOrderedByCreated : AbstractIndexCreationTask<BlogPost, BlogPost>
    {
        public BlogPostsOrderedByCreated()
        {
            this.Map = posts => from post in posts
                                where post.PublishedAt > DateTime.MinValue
                                orderby post.PublishedAt descending
                                select
                                    new BlogPost
                                        {
                                            Author = post.Author,
                                            BlogKey = post.BlogKey,
                                            Tags = post.Tags,
                                            PublishedAt = post.PublishedAt
                                    //        PublishedAt =
                                    //post.PublishedAt.HasValue ? post.PublishedAt.Value.Year : (int?)null,
                                    //        PublishedAt =
                                    //post.PublishedAt.HasValue ? post.PublishedAt.Value.Month : (int?)null
                                        };
        }
    }
}
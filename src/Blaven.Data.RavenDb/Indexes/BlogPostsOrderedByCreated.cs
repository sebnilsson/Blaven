using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.RavenDb.Indexes
{
    public class BlogPostsOrderedByCreated : AbstractIndexCreationTask<BlogPost, BlogPost>
    {
        public BlogPostsOrderedByCreated()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted && post.Published > DateTime.MinValue
                                orderby post.Published descending
                                select
                                    new
                                        {
                                            post.BlogKey,
                                            post.Tags,
                                            Post = post,
                                            post.Published,
                                            Published_Year = post.Published.Year,
                                            Published_Month = post.Published.Month,
                                        };
        }
    }
}
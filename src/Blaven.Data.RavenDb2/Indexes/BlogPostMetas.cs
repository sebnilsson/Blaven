using System;
using System.Linq;

using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2.Indexes
{
    public class BlogPostMetas : AbstractIndexCreationTask<BlogPost, BlogPostMeta>
    {
        public BlogPostMetas()
        {
            this.Map = posts => from post in posts
                                where !post.IsDeleted && post.Published > DateTime.MinValue
                                select new { post.Id, post.BlogKey, post.DataSourceId, post.Checksum, post.Published };
        }
    }
}
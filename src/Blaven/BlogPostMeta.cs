using System;

using Blaven.RavenDb;

namespace Blaven
{
    public class BlogPostMeta
    {
        public BlogPostMeta()
        {
        }

        public BlogPostMeta(string dataSourceId)
        {
            this.SetIds(dataSourceId);
        }

        protected internal BlogPostMeta(BlogPost blogPost)
        {
            this.Id = blogPost.Id;
            this.BlogKey = blogPost.BlogKey;
            this.Checksum = blogPost.Checksum;
            this.DataSourceId = blogPost.DataSourceId;
            this.Published = blogPost.Published;
        }

        public string Id { get; set; }

        public string BlogKey { get; set; }

        public string Checksum { get; set; }

        public string DataSourceId { get; set; }

        public DateTime Published { get; set; }

        internal void SetIds(string dataSourceId)
        {
            this.DataSourceId = dataSourceId;

            var blavenId = BlavenHelper.GetBlavenHash(dataSourceId);
            this.Id = RavenDbHelper.GetEntityId<BlogPost>(blavenId);
        }
    }
}
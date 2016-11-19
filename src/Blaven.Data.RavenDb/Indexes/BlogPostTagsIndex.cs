using System;
using System.Linq;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb.Indexes
{
    public class BlogPostTagsIndex : AbstractIndexCreationTask<BlogPost, BlogPostTagsIndex.Result>
    {
        public BlogPostTagsIndex()
        {
            this.Map = posts => from post in posts
                                where post.PublishedAt > DateTime.MinValue
                                from tag in post.BlogPostTags
                                select
                                new Result
                                    {
                                        BlogKey = post.BlogKey,
                                        BlavenId = post.BlavenId,
                                        PublishedAt = post.PublishedAt,
                                        TagText = tag.Text
                                    };
            
            this.Index(x => x.PublishedAt, FieldIndexing.Default);
            this.Index(x => x.TagText, FieldIndexing.Default);
        }

        public class Result
        {
            public string BlogKey { get; set; }

            public string BlavenId { get; set; }

            public DateTime? PublishedAt { get; set; }

            public string TagText { get; set; }
        }
    }
}
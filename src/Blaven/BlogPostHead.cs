using System;
using System.Collections.Generic;

namespace Blaven
{
    public class BlogPostHead
    {
        public BlogPostHead(BlogPost blogPost)
        {
            BlavenId = blogPost.BlavenId;
            BlogKey = blogPost.BlogKey;
            Published = blogPost.Published;
            Tags = blogPost.Tags;
            Title = blogPost.Title;
            Updated = blogPost.Updated;
            UrlSlug = blogPost.UrlSlug;
        }

        public string BlogKey { get; set; }

        public string BlavenId { get; set; }

        public DateTime Published { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title { get; set; }

        public DateTime? Updated { get; set; }

        public string UrlSlug { get; set; }
    }
}
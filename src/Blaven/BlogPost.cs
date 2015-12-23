using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public class BlogPost : BlogPostBase
    {
        public BlogPost()
        {
            this.Author = new BlogAuthor();
            this.Tags = Enumerable.Empty<string>();
        }

        public BlogAuthor Author { get; set; }

        public string BlavenId { get; set; }

        public string BlogKey { get; set; }

        public string Content { get; set; }

        public string ImageUrl { get; set; }

        public DateTime PublishedAt { get; set; }

        public string SourceUrl { get; set; }

        public string Summary { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
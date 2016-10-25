using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}, Title={Title}")]
    public class BlogPostHead : BlogPostBase
    {
        public BlogAuthor Author { get; set; } = new BlogAuthor();

        public string ImageUrl { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string SourceUrl { get; set; }

        public string Summary { get; set; }

        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();

        public string Title { get; set; }

        public string UrlSlug { get; set; }
    }
}
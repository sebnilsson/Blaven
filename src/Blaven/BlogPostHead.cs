using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}, Title={Title}")]
    public class BlogPostHead : BlogPostBase
    {
        public BlogAuthor Author { get; set; } = new BlogAuthor();

        public string ImageUrl { get; set; }

        public string Summary { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
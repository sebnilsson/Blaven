using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Hash={Hash}, Title={Title}")]
    public class BlogPostHeader
    {
        public BlogAuthor BlogAuthor { get; set; } = new BlogAuthor();

        public string BlogAuthorId { get; set; } = string.Empty;

        public BlogKey BlogKey { get; set; }

        public string Hash { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime? PublishedAt { get; set; }

        public string Slug { get; set; } = string.Empty;

        public string SourceId { get; set; } = string.Empty;

        public string SourceUrl { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public IReadOnlyList<string> Tags { get; set; } = new List<string>(0);

        public string Title { get; set; } = string.Empty;

        public DateTime? UpdatedAt { get; set; }
    }
}

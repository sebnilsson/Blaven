using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Hash={Hash}, Title={Title}")]
    public class BlogPostHeader : BlogPostBase
    {
        public BlogAuthor Author { get; set; } = new BlogAuthor();

        public string ImageUrl { get; set; } = string.Empty;

        public bool IsPublished
            => !IsDraft && PublishedAt <= DateTime.UtcNow;

        public bool IsScheduled
            => IsDraft && PublishedAt > DateTime.UtcNow;

        public DateTimeOffset? PublishedAt { get; set; }

        public string Series { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string SourceUrl { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public IReadOnlyCollection<string> Tags { get; set; }
            = new HashSet<string>();

        public string Title { get; set; } = string.Empty;
    }
}

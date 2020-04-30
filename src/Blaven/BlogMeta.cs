using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Name={Name}")]
    public class BlogMeta
    {
        public BlogKey BlogKey { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DateTimeOffset? PublishedAt { get; set; }

        public string SourceId { get; set; } = string.Empty;

        public DateTimeOffset? UpdatedAt { get; set; }

        public string Url { get; set; } = string.Empty;
    }
}

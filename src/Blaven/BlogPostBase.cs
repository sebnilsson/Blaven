using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Hash={Hash}, Title={Title}")]
    public class BlogPostBase
    {
        public BlogKey BlogKey { get; set; }

        public string Hash { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;

        public string SourceId { get; set; } = string.Empty;

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}

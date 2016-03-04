using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, SourceId={SourceId}, Name={Name}")]
    public class BlogMeta : BlogKeyItemBase
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string SourceId { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Url { get; set; }
    }
}
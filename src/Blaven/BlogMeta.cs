using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, SourceId={SourceId}, Name={Name}")]
    public class BlogMeta
    {
        public string BlogKey { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public DateTime? Published { get; set; }

        public string SourceId { get; set; }

        public DateTime? Updated { get; set; }

        public string Url { get; set; }
    }
}
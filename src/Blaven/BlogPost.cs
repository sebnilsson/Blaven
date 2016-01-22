using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Blaven
{
    [DebuggerDisplay(
        "BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}, "
        + "Title={Title}, Content.Length={Content.Length}")]
    public class BlogPost : BlogPostHead
    {
        public string Content { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string SourceUrl { get; set; }

        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}
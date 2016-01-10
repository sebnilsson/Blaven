using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Hash={Hash}, SourceId={SourceId}")]
    public class BlogPostBase
    {
        public string Hash { get; set; }

        public string SourceId { get; set; }
    }
}
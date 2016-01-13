using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("SourceId={SourceId}, Hash={Hash}")]
    public class BlogPostBase
    {
        public string Hash { get; set; }

        public string SourceId { get; set; }
    }
}
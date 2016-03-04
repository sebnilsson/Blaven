using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}")]
    public class BlogPostBase : BlogKeyItemBase
    {
        public string BlavenId { get; set; }

        public string Hash { get; set; }

        public string SourceId { get; set; }
    }
}
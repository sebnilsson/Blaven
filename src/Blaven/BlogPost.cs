using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay(
        "BlogKey={BlogKey}, BlavenId={BlavenId}, SourceId={SourceId}, Hash={Hash}, "
        + "Title={Title}, Content.Length={Content.Length}")]
    public class BlogPost : BlogPostHead
    {
        public string Content { get; set; }
    }
}
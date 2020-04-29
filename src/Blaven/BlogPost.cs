using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Id={Id}, Hash={Hash}, Title={Title}, " +
        "Content.Length={Content.Length}")]
    public class BlogPost : BlogPostHeader
    {
        public string Content { get; set; } = string.Empty;
    }
}

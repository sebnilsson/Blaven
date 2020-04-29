using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Name={Name}, Count={Count}")]
    public class BlogArchiveItem
    {
        public BlogKey BlogKey { get; set; }

        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

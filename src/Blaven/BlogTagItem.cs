using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Name={Name}, Count={Count}")]
    public class BlogTagItem
    {
        public string BlogKey { get; set; }

        public string Name { get; set; }

        public int Count { get; set; }
    }
}
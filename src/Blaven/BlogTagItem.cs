using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Name={Name}, Count={Count}")]
    public class BlogTagItem : BlogKeyItemBase
    {
        public int Count { get; set; }

        public string Name { get; set; }
    }
}
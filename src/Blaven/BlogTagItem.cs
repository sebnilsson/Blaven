using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Name={Name}, Count={Count}")]
    public class BlogTagItem
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}
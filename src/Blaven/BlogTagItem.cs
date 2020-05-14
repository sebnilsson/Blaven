using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Name={Name}, Count={Count}")]
    public class BlogTagItem
    {
        public int Count { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}

using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Date={Date}, Count={Count}")]
    public class BlogDateItem
    {
        public BlogKey BlogKey { get; set; }

        public int Count { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}

using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Date={Date}, Count={Count}")]
    public class BlogArchiveItem
    {
        public string BlogKey { get; set; }

        public DateTime Date { get; set; }

        public int Count { get; set; }
    }
}
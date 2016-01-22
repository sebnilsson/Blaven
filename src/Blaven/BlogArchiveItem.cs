using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Date={Date}, Count={Count}")]
    public class BlogArchiveItem
    {
        public DateTime Date { get; set; }

        public int Count { get; set; }
    }
}
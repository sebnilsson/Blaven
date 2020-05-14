using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Date={Date}, Count={Count}")]
    public class BlogDateItem
    {
        public int Count { get; set; }

        public DateTimeOffset Date { get; set; }
    }
}

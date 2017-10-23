using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("BlogKey={BlogKey}, Date={Date}, Count={Count}")]
    public class BlogArchiveItem : BlogKeyItemBase
    {
        public int Count { get; set; }

        public DateTime Date { get; set; }
    }
}
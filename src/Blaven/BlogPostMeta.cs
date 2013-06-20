using System;

namespace Blaven
{
    public class BlogPostMeta
    {
        public string Id { get; set; }

        public string BlogKey { get; set; }

        public string Checksum { get; set; }

        public string DataSourceId { get; set; }

        public DateTime Published { get; set; }
    }
}
using System;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerBlogData
    {
        public string Description { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime? Published { get; set; }

        public DateTime? Updated { get; set; }

        public string Url { get; set; }
    }
}
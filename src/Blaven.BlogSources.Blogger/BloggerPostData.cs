using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven.BlogSources.Blogger
{
    [DebuggerDisplay("Id={Id}, Title={Title}, Updated={Updated}")]
    public class BloggerPostData
    {
        public AuthorData Author { get; set; }

        public string Content { get; set; }

        public string ETag { get; set; }

        public string Id { get; set; }

        public ICollection<string> Labels { get; set; }

        public DateTime? Published { get; set; }

        public string Title { get; set; }

        public DateTime? Updated { get; set; }

        public string Url { get; set; }
        
        public class AuthorData
        {
            public string DisplayName { get; set; }

            public string Id { get; set; }

            public ImageData Image { get; set; }

            public string Url { get; set; }

            public class ImageData
            {
                public string Url { get; set; }
            }
        }
    }
}
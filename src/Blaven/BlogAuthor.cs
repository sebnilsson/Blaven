using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("SourceId={SourceId}, Name={Name}, Url={Url}")]
    public class BlogAuthor
    {
        public long Id { get; set; }

        public List<BlogPost> BlogPosts { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public string SourceId { get; set; }

        public string Url { get; set; }
    }
}
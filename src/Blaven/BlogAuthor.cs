using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Id={Id}, Name={Name}, Url={Url}")]
    public class BlogAuthor
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public string ImageUrl { get; set; }

        public string SourceId { get; set; }

        public string Url { get; set; }
    }
}
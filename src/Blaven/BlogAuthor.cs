using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("Id={Id}, Name={Name}, Url={Url}")]
    public class BlogAuthor
    {
        public string Id { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string SourceId { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}

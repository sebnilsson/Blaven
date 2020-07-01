using System;
using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("({Episode}) Title={Title}, IsPublished={IsPublished}")]
    public class BlogSeriesEpisode
    {
        public int Episode { get; set; }

        public string Id { get; set; } = string.Empty;

        public bool IsPublished { get; set; }

        public DateTimeOffset? PublishedAt { get; set; }

        public string Slug { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
    }
}

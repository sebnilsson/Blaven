using System.Diagnostics;

namespace Blaven
{
    [DebuggerDisplay("({Episode}) Title={Title}, IsPublished={IsPublished}")]
    public class BlogPostSeriesEpisode : BlogPostHeader
    {
        public BlogPostSeriesEpisode()
        {
        }

        public BlogPostSeriesEpisode(BlogPost blogPost, int episode)
            : base(blogPost)
        {
            Episode = episode;
        }

        public int Episode { get; set; }
    }
}

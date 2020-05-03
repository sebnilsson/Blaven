using System.Collections.Generic;
using System.Linq;

namespace Blaven.Testing
{
    public class BlogSourceTestConfig
    {
        public BlogSourceTestConfig(
            IEnumerable<BlogPost>? blogSourcePosts = null)
        {
            BlogSourcePosts = blogSourcePosts ?? Enumerable.Empty<BlogPost>();
        }

        public IEnumerable<BlogPost> BlogSourcePosts { get; }
    }
}

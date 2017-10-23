using System.Collections.Generic;
using System.Diagnostics;

namespace Blaven.BlogSources.Blogger
{
    [DebuggerDisplay("NextPageToken={NextPageToken}, Items={Items}")]
    public class BloggerPostsData
    {
        public ICollection<BloggerPostData> Items { get; set; }

        public string NextPageToken { get; set; }
    }
}
using System;
using System.Xml.Linq;

namespace BloggerViewController {
    public interface IBlogStore {
        void Update(XDocument bloggerDocument);
        BlogInfo GetBlogInfo();
        BlogSelection GetBlogSelection(int pageIndex, int? pageSize);
        BlogPost GetBlogPost(string link);
        BlogPost GetBlogPostById(string blogId);
        DateTime? LastUpdate { get; }
    }
}

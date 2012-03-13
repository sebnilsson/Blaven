using System;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    public interface IBlogStore {
        BlogInfo GetBlogInfo(string blogKey);
        BlogPost GetBlogPost(string permaLink, string blogKey);
        BlogSelection GetBlogSelection(int pageIndex, int pageSize, string blogKey, Func<BlogPost, bool> predicate = null);
        bool GetIsBlogUpdated(string blogKey);
        void Update(XDocument bloggerDocument, string blogKey);
    }
}
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    public interface IBlogStore {
        BlogInfo GetBlogInfo(string blogKey);
        BlogPost GetBlogPost(string permaLink, string blogKey);
        BlogSelection GetBlogSelection(int pageIndex, int? pageSize, string blogKey);
        bool GetIsBlogUpdated(string blogKey);
        void Update(XDocument bloggerDocument, string blogKey);
    }
}

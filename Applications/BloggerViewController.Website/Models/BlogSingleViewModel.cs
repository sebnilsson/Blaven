using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloggerViewController.Website.Models {
    public class BlogSingleViewModel {
        public BlogData Data { get; set; }
        public BlogPostDetail Post { get { return Data.Posts.FirstOrDefault(); } }
    }
}
using System.Collections.Generic;

namespace BloggerViewController.Website.Models {
    public class BlogListViewModel {
        public BlogInfo Info { get; set; }
        public BlogSelection Selection { get; set; }
        public int PageIndex { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloggerViewController.Website.Models {
    public class BlogListViewModel {
        public BlogData Data { get; set; }
        public BlogPagingHelper Paging { get; set; }
        public int PageIndex { get; set; }
    }
}
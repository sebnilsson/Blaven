using System;

namespace BloggerViewController.Website.Models {
    public class BlogSingleViewModel {
        public BlogInfo Info { get; set; }

        public Tuple<string, string> NextPost { get; set; }

        public Tuple<string, string> PreviousPost { get; set; }

        public BlogPost Post { get; set; }
    }
}
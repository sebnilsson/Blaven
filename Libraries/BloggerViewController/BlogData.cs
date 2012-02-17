using System;
using System.Collections.Generic;

namespace BloggerViewController {
    public class BlogData {
        public BlogData(bool hasNextItems = false, int? pageSize = 10, int? currentPageIndex = 0) {
            if(pageSize < 1) {
                throw new ArgumentOutOfRangeException("pageSize", "The 'pageSize'-argument has to be a positive number above 0.");
            }
            if(currentPageIndex < 0) {
                throw new ArgumentOutOfRangeException("currentPageIndex", "The 'currentPageIndex'-argument has to be a positive number of 0 or higher.");
            }

            PageIndex = pageSize.Value;
            PageSize = currentPageIndex.Value;
        }
        
        public IEnumerable<string> Categories { get; set; }
        public string Description { get; set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public IEnumerable<BlogPostDetail> Posts { get; set; }
        public string Title { get; set; }
        public DateTime Updated { get; set; }
    }
}

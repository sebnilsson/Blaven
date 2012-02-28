using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogSelection {
        public const int DefaultPageSize = 5;

        public BlogSelection(IEnumerable<BlogPost> allPosts, IEnumerable<BlogPost> selectionPosts, int pageIndex, int? pageSize = DefaultPageSize) {
            if(pageSize < 1) {
                throw new ArgumentOutOfRangeException("pageSize", "The 'pageSize'-argument has to be a positive number above 0.");
            }
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The 'pageIndex'-argument has to be a positive number of 0 or higher.");
            }

            PageIndex = pageIndex;
            PageSize = pageSize.Value;
            Posts = selectionPosts;

            if(!allPosts.Any() || !selectionPosts.Any()) {
                return;
            }

            HasNextItems = (allPosts.LastOrDefault().ID != selectionPosts.LastOrDefault().ID);
            HasPreviousItems = (allPosts.FirstOrDefault().ID != selectionPosts.FirstOrDefault().ID);
        }

        public bool HasNextItems { get; private set; }
        public bool HasPreviousItems { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public IEnumerable<BlogPost> Posts { get; private set; }
    }
}

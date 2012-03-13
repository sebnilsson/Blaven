using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    public class BlogSelection {
        public const int DefaultPageSize = 5;

        public BlogSelection(IEnumerable<BlogPost> selectedPosts, int pageIndex, int? pageSize = DefaultPageSize) {
            if(selectedPosts == null) {
                throw new ArgumentNullException("selectedPosts");
            }
            if(pageSize < 1) {
                throw new ArgumentOutOfRangeException("pageSize", "The argument has to be a positive number above 0.");
            }
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The argument has to be a positive number of 0 or higher.");
            }

            PageIndex = pageIndex;
            PageSize = pageSize.Value;
            
            int take = PageSize;
            int skip = (PageIndex * take);

            var pagedPosts = selectedPosts.Skip(skip).Take(take);                        
            Posts = pagedPosts;

            if(!selectedPosts.Any() || !pagedPosts.Any()) {
                return;
            }

            HasNextItems = (selectedPosts.LastOrDefault().ID != pagedPosts.LastOrDefault().ID);
            HasPreviousItems = (selectedPosts.FirstOrDefault().ID != pagedPosts.FirstOrDefault().ID);
        }

        public bool HasNextItems { get; private set; }
        public bool HasPreviousItems { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public IEnumerable<BlogPost> Posts { get; private set; }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven {
    /// <summary>
    /// A selection of blog-posts, with pagination-information.
    /// </summary>
    public class BlogSelection {
        /// <summary>
        /// Creates an instance of a selection of blog-posts with pagination-information.
        /// </summary>
        /// <param name="blogPosts">The blog-posts to paginate over.</param>
        /// <param name="pageIndex">The current page-index of pagination.</param>
        /// <param name="pageSize">Optional parameter for page-size. Defaults to value in configuration.</param>
        public BlogSelection(IEnumerable<BlogPost> blogPosts, int pageIndex, int? pageSize = null) {
            if(blogPosts == null) {
                throw new ArgumentNullException("selectedPosts");
            }
            pageSize = pageSize.GetValueOrDefault(AppSettingsService.PageSize);
            if(pageSize < 1) {
                throw new ArgumentOutOfRangeException("pageSize", "The argument has to be a positive number above 0.");
            }
            if(pageIndex < 0) {
                throw new ArgumentOutOfRangeException("pageIndex", "The argument has to be a positive number of 0 or higher.");
            }

            PageIndex = pageIndex;
            PageSize = pageSize.Value;
            TotalPostsCount = blogPosts.Count();

            int skip = BlogSelection.GetSkip(PageIndex, PageSize);
            int take = BlogSelection.GetTake(PageSize);

            var pagedPosts = blogPosts.Skip(skip).Take(take);                        
            Posts = pagedPosts;

            if(!blogPosts.Any() || !pagedPosts.Any()) {
                return;
            }

            HasNextItems = (blogPosts.LastOrDefault().ID != pagedPosts.LastOrDefault().ID);
            HasPreviousItems = (blogPosts.FirstOrDefault().ID != pagedPosts.FirstOrDefault().ID);
        }

        public static int GetSkip(int pageIndex, int pageSize) {
            return (pageIndex * pageSize);
        }

        public static int GetTake(int pageSize) {
            return pageSize;
        }

        /// <summary>
        /// Gets if the selection has more items next in the pagination.
        /// </summary>
        public bool HasNextItems { get; private set; }

        /// <summary>
        /// Gets if the selection has more items previous in the pagination.
        /// </summary>
        public bool HasPreviousItems { get; private set; }

        /// <summary>
        /// Gets the current page-index of the pagination.
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the page-size of the pagination.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the current posts in the pagination.
        /// </summary>
        public IEnumerable<BlogPost> Posts { get; private set; }

        /// <summary>
        /// Gets the total count of posts, which are paginated over in the object.
        /// </summary>
        public int TotalPostsCount { get; set; }
    }
}
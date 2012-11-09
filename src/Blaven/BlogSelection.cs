using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    /// <summary>
    /// A selection of blog-posts, with pagination-information.
    /// </summary>
    public class BlogSelection
    {
        /// <summary>
        /// Creates an instance of a selection of blog-posts with pagination-information.
        /// </summary>
        /// <param name="blogPosts">The blog-posts to paginate over.</param>
        /// <param name="pageIndex">The current page-index of pagination.</param>
        /// <param name="pageSize">Optional parameter for page-size. Defaults to value in configuration.</param>
        public BlogSelection(IEnumerable<BlogPost> blogPosts, int pageIndex, int? pageSize = null)
        {
            if (blogPosts == null)
            {
                throw new ArgumentNullException("blogPosts");
            }
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "pageIndex", "The argument has to be a positive number of 0 or higher.");
            }
            pageSize = pageSize.GetValueOrDefault(AppSettingsService.PageSize);
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "The argument has to be a positive number above 0.");
            }

            try
            {
                SetFields(blogPosts, pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                if (ex.Source == "Raven.Database")
                {
                    throw new BlogServiceNotInitException(ex);
                }
                throw;
            }
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
        /// Gets the count of pages. Based on TotalPostCount and PageSize.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Gets the total count of posts, which are paginated over in the object.
        /// </summary>
        public int TotalPostCount { get; set; }

        public void SetFields(IEnumerable<BlogPost> blogPosts, int pageIndex, int? pageSize = null)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize.Value;

            this.TotalPostCount = blogPosts.Count();
            this.PageCount = this.TotalPostCount / this.PageSize;

            int skip = BlogSelection.GetSkip(PageIndex, PageSize);
            int take = BlogSelection.GetTake(PageSize);

            var pagedPosts = blogPosts.Skip(skip).Take(take).ToList();
            this.Posts = pagedPosts;

            if (!blogPosts.Any() || !pagedPosts.Any())
            {
                return;
            }

            this.HasNextItems = (blogPosts.LastOrDefault().Id != pagedPosts.LastOrDefault().Id);
            this.HasPreviousItems = (blogPosts.FirstOrDefault().Id != pagedPosts.FirstOrDefault().Id);
        }

        public static int GetSkip(int pageIndex, int pageSize)
        {
            return (pageIndex * pageSize);
        }

        public static int GetTake(int pageSize)
        {
            return pageSize;
        }
    }
}
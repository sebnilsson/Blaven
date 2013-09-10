using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb;
using Blaven.Transformers;

namespace Blaven
{
    /// <summary>
    /// A collection of blog-posts, with pagination-information.
    /// </summary>
    public class BlogPostCollection
    {
        /// <summary>
        /// Creates an instance of BlogPostCollection.
        /// </summary>
        /// <param name="ravenQuery">The blog-posts to paginate over.</param>
        /// <param name="pageIndex">The current page-index of pagination.</param>
        /// <param name="pageSize">Optional parameter for page-size. Defaults to value in configuration.</param>
        public BlogPostCollection(IQueryable<BlogPost> ravenQuery, int pageIndex, int? pageSize = null)
        {
            if (ravenQuery == null)
            {
                throw new ArgumentNullException("ravenQuery");
            }
            if (pageIndex < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "pageIndex", "The argument has to be a positive number of 0 or higher.");
            }

            pageSize = pageSize ?? AppSettingsService.PageSize;
            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize", "The argument has to be a positive number above 0.");
            }

            RavenDbHelper.HandleRavenExceptions(() => this.SetFields(ravenQuery, pageIndex, pageSize.Value));
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
        public int PageCount { get; private set; }

        /// <summary>
        /// Gets the total count of posts, which are paginated over in the object.
        /// </summary>
        public int TotalPostCount { get; private set; }

        public BlogPostCollection ApplyTransformers(BlogPostTransformersCollection transformers)
        {
            if (transformers != null)
            {
                transformers.ApplyTransformers(this.Posts);
            }
            return this;
        }

        private void SetFields(IQueryable<BlogPost> queryBlogPosts, int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;

            this.TotalPostCount = queryBlogPosts.Count();
            this.PageCount = this.TotalPostCount / this.PageSize;

            int skip = GetSkip(PageIndex, PageSize);
            int take = this.PageSize;

            var pagedPosts = queryBlogPosts.Skip(skip).Take(take).ToList();
            this.Posts = pagedPosts;

            if (!queryBlogPosts.Any() || !pagedPosts.Any())
            {
                return;
            }

            this.HasNextItems = (queryBlogPosts.LastOrDefault().Id != pagedPosts.LastOrDefault().Id);
            this.HasPreviousItems = (queryBlogPosts.FirstOrDefault().Id != pagedPosts.FirstOrDefault().Id);
        }

        private static int GetSkip(int pageIndex, int pageSize)
        {
            return (pageIndex * pageSize);
        }
    }
}
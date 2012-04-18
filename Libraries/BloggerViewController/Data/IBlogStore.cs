using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    /// <summary>
    /// Defines the interface for a store of blog-data.
    /// </summary>
    public interface IBlogStore {
        /// <summary>
        /// Gets the info from a blog by the given key.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info from.</param>
        /// <returns>Returns blog-info.</returns>
        BlogInfo GetBlogInfo(string blogKey);

        /// <summary>
        /// Gets all the labels of a blog, with the count of each label.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the labels from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and count as values.</returns>
        Dictionary<string, int> GetBlogLabels(string blogKey);

        /// <summary>
        /// Gets the last update of the blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the last update-date from.</param>
        /// <returns>Returns a nullable DateTime of the last update, that will be null if there is no update recorded.</returns>
        DateTime? GetBlogLastUpdate(string blogKey);

        /// <summary>
        /// Gets a blog-post by perma-link and blog-key.
        /// </summary>
        /// <param name="permaLinkRelative">The relative permaLink of the blog-post.</param>
        /// <param name="blogKey">The key of the blog containing the post.</param>
        /// <returns>Returns a blog-post.</returns>
        BlogPost GetBlogPost(string blogKey, string permaLink);

        /// <summary>
        /// Gets all the blog post-dates, grouped by date, containing the count of the blog-posts.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the blog post-dates from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and the blog-posts as values.</returns>
        Dictionary<DateTime, int> GetBlogPostDates(string blogKey);

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info from a blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the selection from.</param>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        BlogSelection GetBlogSelection(string blogKey, int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null);
        
        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info from all blogs in store.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        BlogSelection GetBlogSelection(int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null);

        /// <summary>
        /// Checks if the blog in the store is updated.
        /// </summary>
        /// <param name="blogKey">The key of the blog to check.</param>
        /// <returns>Returns a boolean indicating if the blog is up to date.</returns>
        bool GetIsBlogUpdated(string blogKey);
        
        /// <summary>
        /// Updates the specified blog with the given Blogger XML-document.
        /// </summary>
        /// <param name="blogKey">The key of the blog to update.</param>
        /// <param name="bloggerDocument">The Blogger XML-document.</param>
        void Update(string blogKey, XDocument bloggerDocument);
    }
}
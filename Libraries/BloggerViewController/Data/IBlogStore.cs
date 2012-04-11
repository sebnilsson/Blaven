using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    /// <summary>
    /// Defines the interface for a store of blog-data.
    /// </summary>
    public interface IBlogStore {
        /// <summary>
        /// Returns info from a blog by the given key.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info from.</param>
        /// <returns>Returns blog-info.</returns>
        BlogInfo GetBlogInfo(string blogKey);

        /// <summary>
        /// Gets a blog-post by perma-link and blog-key.
        /// </summary>
        /// <param name="permaLinkRelative">The relative permaLink of the blog-post.</param>
        /// <param name="blogKey">The key of the blog containing the post.</param>
        /// <returns>Returns a blog-post.</returns>
        BlogPost GetBlogPost(string permaLink, string blogKey);

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <param name="blogKeys">A list of keys of the blogs to get the selection from.</param>
        /// <param name="predicate">Optional predicate to filter blog-posts.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        BlogSelection GetBlogSelection(int pageIndex, int pageSize, IEnumerable<string> blogKeys, Func<BlogPost, bool> predicate = null);

        /// <summary>
        /// Checks if the blog in the store is updated.
        /// </summary>
        /// <param name="blogKey">The key of the blog to check.</param>
        /// <returns>Returns a boolean indicating if the blog is up to date.</returns>
        bool GetIsBlogUpdated(string blogKey);
        
        /// <summary>
        /// Updates the specified blog with the given Blogger XML-document.
        /// </summary>
        /// <param name="bloggerDocument">The Blogger XML-document.</param>
        /// <param name="blogKey">The key of the blog to update.</param>
        void Update(XDocument bloggerDocument, string blogKey);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace BloggerViewController.Data {
    /// <summary>
    /// A store for blog-data that stores the data in memory. Implements IBlogStore.
    /// </summary>
    public class MemoryBlogStore : IBlogStore {
        private static Dictionary<string, BlogData> _blogData = new Dictionary<string, BlogData>();
        private static Dictionary<string, DateTime> _blogUpdates = new Dictionary<string, DateTime>();

        private static BlogData GetBlogData(string blogKey) {
            BlogData data;
            if(!_blogData.TryGetValue(blogKey, out data)) {
                return null;
            }
            return data;
        }

        /// <summary>
        /// Gets info from a blog by the given key.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info from.</param>
        /// <returns>Returns blog-info.</returns>
        public BlogInfo GetBlogInfo(string blogKey) {
            return GetBlogData(blogKey).Info;
        }

        /// <summary>
        /// Gets a blog-post by perma-link and blog-key.
        /// </summary>
        /// <param name="permaLinkRelative">The relative permaLink of the blog-post.</param>
        /// <param name="blogKey">The key of the blog containing the post.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetBlogPost(string permaLinkRelative, string blogKey) {
            var data = GetBlogData(blogKey);
            return data.Posts.FirstOrDefault(post => post.PermaLinkRelative == permaLinkRelative);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <param name="blogKeys">A list of keys of the blogs to get the selection from.</param>
        /// <param name="predicate">Optional predicate to filter blog-posts.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, IEnumerable<string> blogKeys, Func<BlogPost, bool> predicate = null) {
            var blogDatas = _blogData.Values.Where(data => blogKeys.Contains(data.Info.BlogKey));

            var selectedPosts = Enumerable.Empty<BlogPost>();
            foreach(var data in blogDatas) {
                selectedPosts = selectedPosts.Union(data.Posts);
            }

            if(predicate != null) {
                selectedPosts = selectedPosts.Where(predicate);
            }

            selectedPosts = selectedPosts.OrderByDescending(post => post.Published);

            return new BlogSelection(selectedPosts, pageIndex, pageSize);
        }

        /// <summary>
        /// Checks if the blog in the store is updated.
        /// </summary>
        /// <param name="blogKey">The key of the blog to check.</param>
        /// <returns>Returns a boolean indicating if the blog is up to date.</returns>
        public bool GetIsBlogUpdated(string blogKey) {
            if(!_blogUpdates.ContainsKey(blogKey)) {
                return false;
            }

            var updated = _blogUpdates[blogKey];
            return updated.AddMinutes(ConfigurationService.CacheTime) > DateTime.Now;
        }

        /// <summary>
        /// Updates the specified blog with the given Blogger XML-document.
        /// </summary>
        /// <param name="bloggerDocument">The Blogger XML-document.</param>
        /// <param name="blogKey">The key of the blog to update.</param>
        public void Update(XDocument bloggerDocument, string blogKey) {
            _blogUpdates[blogKey] = DateTime.Now;

            if(bloggerDocument == null) {
                return;
            }

            var newBlogData = BloggerHelper.ParseBlogData(bloggerDocument, blogKey);

            _blogData[blogKey] = newBlogData;
        }
    }
}

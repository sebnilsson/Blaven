using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Configuration;

namespace BloggerViewController.Data {
    /// <summary>
    /// A store for blog-data that stores the data in memory. Implements IBlogStore.
    /// </summary>
    public class MemoryBlogStore : IBlogStore {
        private static Dictionary<string, BlogData> _blogData = new Dictionary<string, BlogData>();
        private static Dictionary<string, DateTime> _blogUpdates = new Dictionary<string, DateTime>();

        /// <summary>
        /// Gets info from a blog by the given key.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info from.</param>
        /// <returns>Returns blog-info.</returns>
        public BlogInfo GetBlogInfo(string blogKey) {
            return GetBlogData(blogKey).Info;
        }

        /// <summary>
        /// Gets all the labels of a blog, with the count of each label.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the labels from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and count as values.</returns>
        public Dictionary<string, int> GetBlogLabels(string blogKey) {
            return GetLabels(blogKey);
        }

        /// <summary>
        /// Gets the last update of the blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the last update-date from.</param>
        /// <returns>Returns a nullable DateTime of the last update, that will be null if there is no update recorded.</returns>
        public DateTime? GetBlogLastUpdate(string blogKey) {
            if(!_blogUpdates.ContainsKey(blogKey)) {
                return null;
            }

            return _blogUpdates[blogKey];
        }

        /// <summary>
        /// Gets a blog-post by perma-link and blog-key.
        /// </summary>
        /// <param name="blogKey">The key of the blog containing the post.</param>
        /// <param name="permaLinkRelative">The relative permaLink of the blog-post.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetBlogPost(string blogKey, string permaLinkRelative) {
            var data = GetBlogData(blogKey);
            return data.Posts.FirstOrDefault(post => post.PermaLinkRelative == permaLinkRelative);
        }

        /// <summary>
        /// Gets all the blog post-dates, grouped by date, containing the blog-posts..
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the blog post-dates from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and the blog-posts as values.</returns>
        public Dictionary<DateTime, int> GetBlogPostDates(string blogKey) {
            return GetPostDates(blogKey);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the selection from.</param>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetBlogSelection(string blogKey, int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null) {
            var data = GetBlogData(blogKey);

            var selectedPosts = data.Posts.OrderByDescending(post => post.Published).AsEnumerable();
            if(labelFilter != null) {
                selectedPosts = selectedPosts.Where(post => post.Labels.Any(label => label.Equals(labelFilter, StringComparison.InvariantCultureIgnoreCase)));
            }
            if(dateTimeFilter.HasValue) {
                selectedPosts = selectedPosts.Where(post => post.Published.Year == dateTimeFilter.Value.Year && post.Published.Month == dateTimeFilter.Value.Month);
            }

            return new BlogSelection(selectedPosts, pageIndex, pageSize);
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info from all blogs in store.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null) {
            var allBlogKeys = _blogData.Keys;

            var selectedPosts = Enumerable.Empty<BlogPost>();
            foreach(var blogKey in allBlogKeys) {
                var data = GetBlogData(blogKey);
                var posts = data.Posts;
                if(labelFilter != null) {
                    posts = posts.Where(post => post.Labels.Any(label => label.Equals(labelFilter, StringComparison.InvariantCultureIgnoreCase)));
                }
                if(dateTimeFilter.HasValue) {
                    posts = posts.Where(post => post.Published.Year == dateTimeFilter.Value.Year && post.Published.Month == dateTimeFilter.Value.Month);
                }

                selectedPosts = selectedPosts.Concat(posts);
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
            var lastUpdate = GetBlogLastUpdate(blogKey);
            if(!lastUpdate.HasValue) {
                return false;
            }
            
            return lastUpdate.Value.AddMinutes(AppSettingsService.CacheTime) > DateTime.Now;
        }

        /// <summary>
        /// Updates the specified blog with the given Blogger XML-document.
        /// </summary>
        /// <param name="blogKey">The key of the blog to update.</param>
        /// <param name="bloggerDocument">The Blogger XML-document.</param>
        public void Update(string blogKey, System.Xml.Linq.XDocument bloggerDocument) {
            var parsedData = BloggerHelper.ParseBlogData(blogKey, bloggerDocument);

            _blogData[blogKey] = parsedData;

            _blogUpdates[blogKey] = DateTime.Now;
        }

        private static BlogData GetBlogData(string blogKey) {
            BlogData data;
            if(!_blogData.TryGetValue(blogKey, out data)) {
                throw new ArgumentOutOfRangeException("blogKey", string.Format("No blog-data available for blog-key '{0}'.", blogKey));
            }

            return data;
        }

        private Dictionary<string, int> GetLabels(string blogKey) {
            var data = GetBlogData(blogKey);

            var labels = new Dictionary<string, int>();
            foreach(var post in data.Posts) {
                foreach(var label in post.Labels) {
                    if(!labels.ContainsKey(label)) {
                        labels.Add(label, 0);
                    }
                    labels[label] = (labels[label] + 1);
                }
            }
            return labels;
        }

        private Dictionary<DateTime, int> GetPostDates(string blogKey) {
            var data = GetBlogData(blogKey);

            var postDates = new Dictionary<DateTime, int>();
            foreach(var post in data.Posts) {
                DateTime key = new DateTime(post.Published.Year, post.Published.Month, 1);
                if(!postDates.ContainsKey(key)) {
                    postDates.Add(key, 0);
                }
                postDates[key] = (postDates[key] + postDates[key]);
            }
            return postDates;
        }
    }
}

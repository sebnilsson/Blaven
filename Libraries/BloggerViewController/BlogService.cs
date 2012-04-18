using System;
using System.Collections.Generic;
using System.Linq;

namespace BloggerViewController {
    /// <summary>
    /// A service-class for accessing blog-related features.
    /// </summary>
    public class BlogService {
        /// <summary>
        /// Creates an instance of a service-class for accessing blog-related features.
        /// </summary>
        /// <param name="config">The Blogger-settings to use in the service.</param>
        public BlogService(BlogServiceConfig config) {
            if(config == null) {
                throw new ArgumentNullException("config");
            }

            this.Config = config;
        }

        /// <summary>
        /// Gets the configuration being used by the service.
        /// </summary>
        public BlogServiceConfig Config { get; private set; }

        private Dictionary<string, BlogServiceBlog> _blogCache = new Dictionary<string, BlogServiceBlog>();

        /// <summary>
        /// Gets a blog from a given blog-key.
        /// </summary>
        /// <param name="blogKey">The key of the blog desired.</param>
        /// <returns>Returns another service-object specific to the blog.</returns>
        public BlogServiceBlog GetBlog(string blogKey) {
            if(!_blogCache.ContainsKey(blogKey)) {
                var bloggerSetting = this.Config.BloggerSettings.FirstOrDefault(setting => setting.BlogKey == blogKey);
                if(bloggerSetting == null) {
                    throw new IndexOutOfRangeException(string.Format("No blog found in settings with key '{0}'.", blogKey));
                }

                _blogCache[blogKey] = new BlogServiceBlog(bloggerSetting, this.Config);
            }

            return _blogCache[blogKey];
        }

        /// <summary>
        /// Gets the first blog in the Blogger-settings.
        /// </summary>
        /// <returns>Returns another service-object specific to the blog.</returns>
        public BlogServiceBlog GetDefaultBlog() {
            var firstBlogKey = this.Config.BloggerSettings.FirstOrDefault().BlogKey;
            return GetBlog(firstBlogKey);
        }

        public BlogSelection GetAllBlogsSelection(int pageIndex, string labelFilter = null, DateTime? dateTimeFilter = null) {
            return this.Config.BlogStore.GetBlogSelection(pageIndex, this.Config.PageSize, labelFilter, dateTimeFilter);
        }

        public Dictionary<string, int> GetAllBlogsLabels() {
            var allBlogLabels = new Dictionary<string, int>();

            this.Config.BloggerSettings.Select(setting => GetBlog(setting.BlogKey)).ToList().ForEach(service => {
                var labels = service.GetLabels();

                foreach(var label in labels) {
                    if(!allBlogLabels.ContainsKey(label.Key)) {
                        allBlogLabels[label.Key] = 0;
                    }

                    allBlogLabels[label.Key] = (allBlogLabels[label.Key] + label.Value);
                }
            });

            return allBlogLabels.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public Dictionary<DateTime, int> GetAllBlogsPostDates() {
            var allBlogPostDates = new Dictionary<DateTime, int>();

            this.Config.BloggerSettings.Select(setting => GetBlog(setting.BlogKey)).ToList().ForEach(serivce => {
                var postDates = serivce.GetPostDates();

                foreach(var postDate in postDates) {
                    if(!allBlogPostDates.ContainsKey(postDate.Key)) {
                        allBlogPostDates[postDate.Key] = 0;
                    }

                    allBlogPostDates[postDate.Key] = (allBlogPostDates[postDate.Key] + postDate.Value);
                }
            });

            return allBlogPostDates.OrderByDescending(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public void UpdateAllBlogs() {
            this.Config.BloggerSettings.Select(setting => GetBlog(setting.BlogKey)).ToList()
                .ForEach(service => service.Update());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Blogger;
using Raven.Client;
using Raven.Client.Document;

namespace BloggerViewController.Data {
    /// <summary>
    /// A store for blog-data that stores the data in a RavenDB-store. Implements IBlogStore.
    /// </summary>
    public class RavenDbBlogStore : IBlogStore {
        private IDocumentStore _documentStore;

        public RavenDbBlogStore(IDocumentStore documentStore = null) {
            _documentStore = documentStore ?? new DocumentStore { Url = ConfigurationService.RavenDbStoreUrlKey };
            _documentStore.Initialize();
        }

        private BlogData GetBlogData(string blogKey) {
            using(var session = _documentStore.OpenSession()) {
                var blogData = session.Load<BlogData>(GetKey<BlogData>(blogKey));
                if(blogData == null) {
                    throw new ArgumentOutOfRangeException("blogKey", string.Format("No blog-data available for blog-key '{0}'.", blogKey));
                }

                return blogData;
            }
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
        public BlogPost GetBlogPost(string permaLink, string blogKey) {
            var data = GetBlogData(blogKey);
            return data.Posts.FirstOrDefault(post => post.PermaLinkRelative == permaLink);
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
            var blogDatas = blogKeys.Select(key => GetBlogData(key));

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
            using(var session = _documentStore.OpenSession()) {
                var storeUpdate = session.Load<BlogStoreUpdate>(GetKey<BlogStoreUpdate>(blogKey));
                if(storeUpdate == null) {
                    return false;
                }

                return storeUpdate.Updated.AddMinutes(ConfigurationService.CacheTime) > DateTime.Now;
            }
        }

        /// <summary>
        /// Updates the specified blog with the given Blogger XML-document.
        /// </summary>
        /// <param name="bloggerDocument">The Blogger XML-document.</param>
        /// <param name="blogKey">The key of the blog to update.</param>
        public void Update(System.Xml.Linq.XDocument bloggerDocument, string blogKey) {
            using(var session = _documentStore.OpenSession()) {
                var parsedData = BloggerHelper.ParseBlogData(bloggerDocument, blogKey);

                string blogDataUrl = GetKey<BlogData>(blogKey);
                string storeUpdateUrl = GetKey<BlogStoreUpdate>(blogKey);

                var blogData = session.Load<BlogData>(blogDataUrl);
                if(blogData == null) {
                    blogData = new BlogData();
                    session.Store(blogData, blogDataUrl);
                }
                blogData.Info = parsedData.Info;
                blogData.Posts = parsedData.Posts;

                var storeUpdate = session.Load<BlogStoreUpdate>(storeUpdateUrl);
                if(storeUpdate == null) {
                    storeUpdate = new BlogStoreUpdate { BlogKey = blogKey };
                    session.Store(storeUpdate, storeUpdateUrl);
                }
                storeUpdate.Updated = DateTime.Now;

                session.SaveChanges();
            }
        }

        private static string GetKey<T>(string key) {
            string typeName = typeof(T).Name;
            return string.Format("{0}/{1}", typeName.ToLowerInvariant(), key);
        }
    }
}

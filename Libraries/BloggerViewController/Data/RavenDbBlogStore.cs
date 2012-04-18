using System;
using System.Collections.Generic;
using System.Linq;

using BloggerViewController.Configuration;
using BloggerViewController.Data.Indexes;
using Raven.Client;
using Raven.Client.Document;

namespace BloggerViewController.Data {
    /// <summary>
    /// A store for blog-data that stores the data in a RavenDB-store. Implements IBlogStore.
    /// </summary>
    public class RavenDbBlogStore : IBlogStore {
        public RavenDbBlogStore(IDocumentStore documentStore = null) {
            Store = documentStore ?? new DocumentStore { Url = AppSettingsService.RavenDbStoreUrlKey };
            Store.Initialize();
        }
        
        public IDocumentStore Store { get; private set; }

        /// <summary>
        /// Gets info from a blog by the given key.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get info from.</param>
        /// <returns>Returns blog-info.</returns>
        public BlogInfo GetBlogInfo(string blogKey) {
            using(var session = Store.OpenSession()) {
                var blogInfo = session.Load<BlogInfo>(GetKey<BlogInfo>(blogKey));
                if(blogInfo == null) {
                    throw new ArgumentOutOfRangeException("blogKey", string.Format("No blog-info was found for blog-key '{0}'.", blogKey));
                }

                return blogInfo;
            }
        }

        /// <summary>
        /// Gets all the labels of a blog, with the count of each label.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the labels from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and count as values.</returns>
        public Dictionary<string, int> GetBlogLabels(string blogKey) {
            using(var session = Store.OpenSession()) {
                var labels = session.Query<LabelsCountByBlogKey.ReduceResult, LabelsCountByBlogKey>().Where(x => x.BlogKey == blogKey)
                                 .ToDictionary(x=> x.Label, x => x.Count);
                return labels;

                /*var labels = (from post in session.Query<BlogPost>().Where(p => p.BlogKey == blogKey).ToList()
                              from label in post.Labels
                              select label).ToList();

                var groupedLabels = (from label in labels
                                     group label by label into labelGrouping
                                     select new {
                                         Label = labelGrouping.Key,
                                         Count = labelGrouping.Count(),
                                     }).ToDictionary(kvp => kvp.Label, kvp => kvp.Count);

                return groupedLabels;*/
            }
        }

        /// <summary>
        /// Gets the last update of the blog.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the last update-date from.</param>
        /// <returns>Returns a nullable DateTime of the last update, that will be null if there is no update recorded.</returns>
        public DateTime? GetBlogLastUpdate(string blogKey) {
            using(var session = Store.OpenSession()) {
                var storeUpdate = session.Load<BlogStoreUpdate>(GetKey<BlogStoreUpdate>(blogKey));
                if(storeUpdate == null) {
                    return null;
                }

                return storeUpdate.Updated;
            }
        }

        /// <summary>
        /// Gets a blog-post by perma-link and blog-key.
        /// </summary>
        /// <param name="blogKey">The key of the blog containing the post.</param>
        /// <param name="permaLinkRelative">The relative permaLink of the blog-post.</param>
        /// <returns>Returns a blog-post.</returns>
        public BlogPost GetBlogPost(string blogKey, string permaLink) {
            using(var session = Store.OpenSession()) {
                var blogPost = session.Query<BlogPost>().FirstOrDefault(post => post.BlogKey == blogKey && post.PermaLinkRelative == permaLink);
                if(blogPost == null) {
                    throw new ArgumentOutOfRangeException("blogKey",
                        string.Format("No blog-post was found for blog-key '{0}' and permaLink '{1}'.", blogKey, permaLink));
                }

                return blogPost;
            }
        }

        /// <summary>
        /// Gets all the blog post-dates, grouped by date, containing the blog-posts..
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the blog post-dates from.</param>
        /// <returns>Returns a dictionary with the label-names as keys and the blog-posts as values.</returns>
        public Dictionary<DateTime, int> GetBlogPostDates(string blogKey) {
            using(var session = Store.OpenSession()) {
                var postDates = session.Query<PostDatesByBlogKey.ReduceResult, PostDatesByBlogKey>().Where(x => x.BlogKey == blogKey)
                    .ToDictionary(x => x.Date, x => x.Count);
                return postDates;

                /*var posts = session.Query<BlogPost>().Where(p => p.BlogKey == blogKey).Select(post => post).ToList();
                                 //let postDate = new DateTime(post.Published.Year, post.Published.Month, 1)
                                 //select new { Date = postDate, Post = post }).ToList();

                var groupedPostDates = (from post in posts
                                        let postDate = new DateTime(post.Published.Year, post.Published.Month, 1)
                                        group post by postDate into postDateGrouping
                                        select new {
                                            Date = postDateGrouping.Key,
                                            Blogs = postDateGrouping.AsEnumerable() ?? Enumerable.Empty<BlogPost>(),
                                        }).OrderBy(postDate => postDate.Date).ToDictionary(kvp => kvp.Date, kvp => kvp.Blogs);

                return groupedPostDates;*/
            }
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info.
        /// </summary>
        /// <param name="blogKey">The key of the blog to get the selection from.</param>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetBlogSelection(string blogKey, int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null) {
            using(var session = Store.OpenSession()) {
                var posts = session.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(result => result.BlogKey == blogKey);
                if(labelFilter != null) {
                    posts = posts.Where(post => post.Labels.Any(label => label.Equals(labelFilter, StringComparison.InvariantCultureIgnoreCase)));
                }
                if(dateTimeFilter.HasValue) {
                    posts = posts.Where(post => post.Published.Year == dateTimeFilter.Value.Year && post.Published.Month == dateTimeFilter.Value.Month);
                }

                posts = posts.OrderByDescending(post => post.Published);

                return new BlogSelection(posts.ToList(), pageIndex, pageSize);
            }
        }

        /// <summary>
        /// Gets a selection of blog-posts, with pagination-info from all blogs in store.
        /// </summary>
        /// <param name="pageIndex">The current page-index of the pagination.</param>
        /// <param name="pageSize">The page-size of the pagination.</param>
        /// <returns>Returns a blog-selection with pagination-info.</returns>
        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, string labelFilter = null, DateTime? dateTimeFilter = null) {
            using(var session = Store.OpenSession()) {
                var posts = session.Query<BlogPost, BlogPostsOrderedByCreated>().AsQueryable();
                if(labelFilter != null) {
                    posts = posts.Where(post => post.Labels.Any(label => label.Equals(labelFilter, StringComparison.InvariantCultureIgnoreCase)));
                }
                if(dateTimeFilter.HasValue) {
                    posts = posts.Where(post => post.Published.Year == dateTimeFilter.Value.Year && post.Published.Month == dateTimeFilter.Value.Month);
                }

                return new BlogSelection(posts, pageIndex, pageSize);
            }
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

            using(var session = Store.OpenSession()) {
                string blogInfoUrl = GetKey<BlogInfo>(blogKey);
                var blogInfo = session.Load<BlogInfo>(blogInfoUrl);
                if(blogInfo == null) {
                    blogInfo = new BlogInfo { BlogKey = blogKey };
                    session.Store(blogInfo, blogInfoUrl);
                }

                blogInfo.Subtitle = parsedData.Info.Subtitle;
                blogInfo.Title = parsedData.Info.Title;
                blogInfo.Updated = parsedData.Info.Updated;

                session.SaveChanges();
            }

            var storeBlogPosts = Enumerable.Empty<BlogPost>();
            using(var session = Store.OpenSession()) {
                storeBlogPosts = session.Query<BlogPost>().Where(post => post.BlogKey == blogKey).ToList();
            }

            UpdateBlogPosts(blogKey, storeBlogPosts, parsedData.Posts);

            using(var session = Store.OpenSession()) {
                string storeUpdateUrl = GetKey<BlogStoreUpdate>(blogKey);
                var storeUpdate = session.Load<BlogStoreUpdate>(storeUpdateUrl);
                if(storeUpdate == null) {
                    storeUpdate = new BlogStoreUpdate { BlogKey = blogKey };
                    session.Store(storeUpdate, storeUpdateUrl);
                }
                storeUpdate.Updated = DateTime.Now;

                session.SaveChanges();
            }
        }

        private void UpdateBlogPosts(string blogKey, IEnumerable<BlogPost> storeBlogPosts, IEnumerable<BlogPost> newParsedPosts) {
            var newPosts = newParsedPosts.Where(parsed => !storeBlogPosts.Any(post => post.ID == parsed.ID));
            foreach(var newPost in newPosts) {
                string postKey = GetKey<BlogPost>(newPost.ID);
                using(var session = Store.OpenSession()) {
                    session.Store(newPost, postKey);
                    session.SaveChanges();
                }
            }

            var updatedPosts = newParsedPosts.Where(parsed => storeBlogPosts.Any(post => post.ID == parsed.ID && post.Updated != parsed.Updated));
            foreach(var updatedPost in updatedPosts) {
                string postKey = GetKey<BlogPost>(updatedPost.ID);
                using(var session = Store.OpenSession()) {
                    var blogPost = session.Load<BlogPost>(postKey);
                    blogPost.Author = updatedPost.Author;
                    blogPost.Content = updatedPost.Content;
                    blogPost.Labels = updatedPost.Labels;
                    blogPost.Title = updatedPost.Title;
                    blogPost.Updated = updatedPost.Updated;
                    
                    session.SaveChanges();
                }
            }
        }

        private static string GetKey<T>(params string[] keys) {
            if(keys == null || !keys.Any()) {
                throw new ArgumentOutOfRangeException("keys", "The provided keys cannot be null or empty.");
            }

            string typeName = typeof(T).Name.ToLowerInvariant();
            string key = string.Join("/", new[] { typeName }.Concat(keys ?? Enumerable.Empty<string>()));
            return key;
        }
    }
}

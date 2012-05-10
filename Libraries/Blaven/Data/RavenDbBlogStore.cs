﻿using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Blogger;
using Blaven.Data.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace Blaven.Data {
    /// <summary>
    /// A store for blog-data that stores the data in a RavenDB-store. Implements IBlogStore.
    /// </summary>
    internal class RavenDbBlogStore {
        public RavenDbBlogStore(IDocumentStore documentStore) {
            DocumentStore = documentStore;
        }
        
        public IDocumentStore DocumentStore { get; private set; }

        public Dictionary<DateTime, int> GetBlogArchiveCount(params string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var archiveCount = session.Query<ArchiveCountByBlogKey.ReduceResult, ArchiveCountByBlogKey>().Where(x => x.BlogKey.In(blogKeys));
                var groupedArchive = from archive in archiveCount.ToList()
                                     group archive by archive.Date into g
                                     select new { Date = g.Key, Count = g.Sum(x => x.Count) };

                return groupedArchive.OrderByDescending(x => x.Date).ToDictionary(x => x.Date, x => x.Count);
            }
        }

        public BlogSelection GetBlogArchiveSelection(DateTime date, int pageIndex, int pageSize, string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var posts = session.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(x => x.BlogKey.In(blogKeys)
                        && x.Published.Year == date.Year && x.Published.Month == date.Month)
                        .OrderByDescending(x => x.Published);

                return new BlogSelection(posts, pageIndex, pageSize);
            }
        }

        public BlogInfo GetBlogInfo(string blogKey) {
            using(var session = DocumentStore.OpenSession()) {
                var blogInfo = session.Load<BlogInfo>(GetKey<BlogInfo>(blogKey));
                if(blogInfo == null) {
                    throw new ArgumentOutOfRangeException("blogKey", string.Format("No blog-info was found for blog-key '{0}'.", blogKey));
                }

                return blogInfo;
            }
        }

        public bool GetIsBlogUpdated(string blogKey, int cacheTimeMinutes) {
            var lastUpdate = GetBlogLastUpdate(blogKey);
            if(!lastUpdate.HasValue) {
                return false;
            }

            return lastUpdate.Value.AddMinutes(cacheTimeMinutes) > DateTime.Now;
        }

        public DateTime? GetBlogLastUpdate(string blogKey) {
            using(var session = DocumentStore.OpenSession()) {
                var storeUpdate = session.Load<BlogStoreUpdate>(GetKey<BlogStoreUpdate>(blogKey));
                if(storeUpdate == null) {
                    return null;
                }

                return storeUpdate.Updated;
            }
        }

        public BlogPost GetBlogPost(string blogKey, string permaLink) {
            using(var session = DocumentStore.OpenSession()) {
                var blogPost = session.Query<BlogPost>().FirstOrDefault(post => post.BlogKey == blogKey && post.PermaLinkRelative == permaLink);
                return blogPost;
            }
        }

        public BlogSelection GetBlogSelection(int pageIndex, int pageSize, params string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var posts = session.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(x => x.BlogKey.In(blogKeys)).OrderByDescending(x => x.Published)
                    .OrderByDescending(x => x.Published);

                return new BlogSelection(posts, pageIndex, pageSize);
            }
        }

        public Dictionary<string, int> GetBlogTagsCount(params string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var tagCount = session.Query<TagsCountByBlogKey.ReduceResult, TagsCountByBlogKey>().Where(x => x.BlogKey.In(blogKeys));
                var tagsGrouped = (from result in tagCount.ToList()
                                   group result by result.Tag into g
                                   select new { Tag = g.Key, Count = g.Sum(x => x.Count), });

                return tagsGrouped.OrderBy(x => x.Tag).ToDictionary(x => x.Tag, x => x.Count);
            }
        }

        public BlogSelection GetBlogTagsSelection(string tagName, int pageIndex, int pageSize, string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var posts = session.Query<BlogPost, BlogPostsOrderedByCreated>()
                    .Where(x => x.BlogKey.In(blogKeys)
                        && x.Tags.Any(tag => tag.Equals(tagName, StringComparison.InvariantCultureIgnoreCase)))
                        .OrderByDescending(x => x.Published);

                return new BlogSelection(posts, pageIndex, pageSize);
            }
        }

        public BlogSelection SearchPosts(string searchTerms, int pageIndex, int pageSize, string[] blogKeys) {
            if(blogKeys == null) {
                throw new ArgumentNullException("blogKeys");
            }

            using(var session = DocumentStore.OpenSession()) {
                var posts = session.Advanced.LuceneQuery<BlogPost, SearchBlogPosts>().Search("Content", searchTerms);

                return new BlogSelection(posts, pageIndex, pageSize);
            }
        }

        public void Update(string blogKey, System.Xml.Linq.XDocument bloggerDocument) {
            var parsedData = BloggerHelper.ParseBlogData(blogKey, bloggerDocument);

            using(var session = DocumentStore.OpenSession()) {
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
            using(var session = DocumentStore.OpenSession()) {
                storeBlogPosts = session.Query<BlogPost>().Where(post => post.BlogKey == blogKey).ToList();
            }

            UpdateBlogPosts(blogKey, storeBlogPosts, parsedData.Posts);

            using(var session = DocumentStore.OpenSession()) {
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
                using(var session = DocumentStore.OpenSession()) {
                    session.Store(newPost, postKey);
                    session.SaveChanges();
                }
            }

            var updatedPosts = newParsedPosts.Where(parsed => storeBlogPosts.Any(post => post.ID == parsed.ID && post.Updated != parsed.Updated));
            foreach(var updatedPost in updatedPosts) {
                string postKey = GetKey<BlogPost>(updatedPost.ID);
                using(var session = DocumentStore.OpenSession()) {
                    var blogPost = session.Load<BlogPost>(postKey);
                    blogPost.Author = updatedPost.Author;
                    blogPost.Content = updatedPost.Content;
                    blogPost.Tags = updatedPost.Tags;
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
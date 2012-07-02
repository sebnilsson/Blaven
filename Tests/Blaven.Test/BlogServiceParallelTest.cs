﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.RavenDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;

namespace Blaven.Test.Integration {
    [TestClass]
    public class BlogServiceParallelTest {
        private readonly IEnumerable<string> _blogKeys = new[] { "buzz_simple", "status_simple", };
        private readonly int _userCount = 3;
        private int _blogCount {
            get { return _blogKeys.Count(); }
        }

        public BlogServiceParallelTest() {
            
        }

        [TestMethod]
        public void ctor_WithEnsureBlogIsRefreshed_StoreShouldContainData() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            Parallel.For(0, 1, (i) => {
                var blogService = GetBlogServiceWithMultipleBlogs(documentStore);
            });

            var blogStore = new RavenDbBlogStore(documentStore);
            var blogsWithDataCount = _blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            var storeDocumentCount = blogStore.DocumentStore.DatabaseCommands.GetStatistics().CountOfDocuments;
            List<StoreBlogRefresh> blogUpdates = null;
            using(var session = blogStore.DocumentStore.OpenSession()) {
                blogUpdates = session.Query<StoreBlogRefresh>().ToList();
            }

            Assert.AreEqual<int>(_blogCount, blogsWithDataCount, "The blogs did not have data at time of query.");
        }

        [TestMethod]
        public void ctor_WithoutEnsureBlogIsRefreshed_StoreShouldNotContainData() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            Parallel.For(0, _userCount, (i) => {
                var blogService = GetBlogServiceWithMultipleBlogs(documentStore, ensureBlogsRefreshed: false);
            });

            var blogStore = new RavenDbBlogStore(documentStore);
            var blogsWithDataCount = _blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            Assert.AreEqual<int>(0, blogsWithDataCount, "The blogs had data, when expected not to.");
        }

        [TestMethod]
        public void Refresh_WithEnsureBlogsRefreshed_FollowingRefreshesShouldNotUpdate() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);
            firstRunBlogService.Config.BlogStore.WaitForIndexes();

            var updatedBlogs = new ConcurrentBag<string>();
            Parallel.For(0, _userCount, (i) => {
                var blogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);
                
                var updated = blogService.Refresh();
                foreach(var update in updated) {
                    updatedBlogs.Add(update);
                }
            });

            Assert.AreEqual<int>(0, updatedBlogs.Count, "The blogs were updated too many times.");
        }

        [TestMethod]
        public void Refresh_WithoutEnsureBlogsRefreshed_FollowingRefreshesShouldUpdate() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore, ensureBlogsRefreshed: false);
            firstRunBlogService.Config.BlogStore.WaitForIndexes();

            var updatedBlogs = new ConcurrentBag<string>();
            Parallel.For(0, _userCount, (i) => {
                var blogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore, ensureBlogsRefreshed: false);

                var refreshResults = blogService.Refresh();
                foreach(var update in refreshResults) {
                    updatedBlogs.Add(update);
                }
            });

            Assert.AreEqual<int>(_blogCount, updatedBlogs.Count, "The blogs weren't updated enough times.");
        }

        private BlogService GetBlogServiceWithMultipleBlogs(IDocumentStore documentStore = null, bool refreshAsync = true, bool ensureBlogsRefreshed = true,
            IEnumerable<string> blogKeys = null) {
            blogKeys = blogKeys ?? _blogKeys;
            documentStore = documentStore ?? DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            return BlogServiceTestHelper.GetBlogService(documentStore, blogKeys, refreshAsync, ensureBlogsRefreshed: ensureBlogsRefreshed);
        }
    }
}
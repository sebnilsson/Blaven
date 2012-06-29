using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;
using Blaven.RavenDb;

namespace Blaven.Test.Integration {
    [TestClass]
    public class BlogServiceTest {
        public BlogServiceTest() {
            
        }

        [TestMethod]
        public void ctor_WithEnsureBlogIsRefreshed_StoreShouldContainData() {
            int blogCount = 2;
            int userCount = 3;

            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("ctor_WithEnsureBlogIsRefreshed_StoreShouldContainData");

            var blogKeys = Enumerable.Empty<string>();
            Parallel.For(0, userCount, (i) => {
                var blogService = GetRefreshTestBlogService(documentStore, refreshAsync: true, ensureBlogsRefreshed: true);

                blogKeys = blogService.Config.BloggerSettings.Select(x => x.BlogKey);
            });
            
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogsWithDataCount = blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            Assert.AreEqual<int>(blogCount, blogsWithDataCount, "The blogs did not have data at time of query.");
        }
        
        [TestMethod]
        public void ctor_WithEnsureBlogIsRefreshedFollowingRefresh_ShouldNotPerformRefresh() {
            int blogCount = 2;
            int userCount = 3;

            var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("ctor_WithEnsureBlogIsRefreshedFollowingRefresh_ShouldNotPerformRefresh");

            var resultsList = new ConcurrentBag<Tuple<string, bool>>();

            Parallel.For(0, userCount, (i) => {
                var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: true, ensureBlogsRefreshed: true);

                var refreshResults = blogService.Refresh();

                foreach(var result in refreshResults) {
                    resultsList.Add(result);
                }
            });

            var unupdatedBlogsCount = resultsList.Where(x => !x.Item2).Count();

            int expectedUnupdatedBlogs = (userCount) * blogCount;

            Assert.AreEqual<int>(expectedUnupdatedBlogs, unupdatedBlogsCount, "Not enough blogs were waited for.");
        }

        [TestMethod]
        public void ctor_WithoutEnsureBlogIsRefreshed_StoreShouldNotContainData() {
            int userCount = 3;

            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("ctor_WithoutEnsureBlogIsRefreshed_StoreShouldNotContainData");

            var blogKeys = Enumerable.Empty<string>();
            Parallel.For(0, userCount, (i) => {
                var blogService = GetRefreshTestBlogService(documentStore, refreshAsync: true, ensureBlogsRefreshed: false);
                
                blogKeys = blogService.Config.BloggerSettings.Select(x => x.BlogKey);
            });

            var blogStore = new RavenDbBlogStore(documentStore);
            var blogsWithDataCount = blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            Assert.AreEqual<int>(0, blogsWithDataCount, "The blogs contained data, when not expected to.");
        }

        [TestMethod]
        public void ctor_WithoutEnsureBlogIsRefreshedFollowingRefresh_ShouldPerformRefreshJustOnceOnEveryBlog() {
            int blogCount = 2;
            int userCount = 3;

            var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("ctor_WithoutEnsureBlogIsRefreshedFollowingRefresh_ShouldPerformRefreshJustOnceOnEveryBlog");

            var resultsList = new ConcurrentBag<Tuple<string, bool>>();

            Parallel.For(0, userCount, (i) => {
                var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: true, ensureBlogsRefreshed: false);

                var refreshResults = blogService.Refresh();

                foreach(var result in refreshResults) {
                    resultsList.Add(result);
                }
            });

            var unupdatedBlogsCount = resultsList.Where(x => !x.Item2).Count();

            int expectedUnupdatedBlogs = (userCount - 1) * blogCount;

            Assert.AreEqual<int>(expectedUnupdatedBlogs, unupdatedBlogsCount, "Not enough blogs were waited for.");
        }
        
        //private static EmbeddableDocumentStore BaseDocumentStore { get; set; }

        //[ClassInitialize()]
        //public static void ClassInitialize(TestContext context) {
        //    BaseDocumentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("BlogServiceTest");
        //}

        //[ClassCleanup()]
        //public static void ClassCleanup() {

        //}

        //[TestMethod]
        //public void Refresh_UsingMultipleCallsWithAsync_ShouldOnlyUpdateOnce() {
        //    var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("Refresh_UsingMultipleCallsWithAsync_ShouldOnlyUpdateOnce");

        //    var resultsList = new ConcurrentBag<string>();
            
        //    Parallel.For(0, 1, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: true);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Assert.AreEqual<int>(2, resultsList.Count);
        //}

        //[TestMethod]
        //public void Refresh_UsingMultipleCallsWithoutAsync_ShouldOnlyUpdateOnce() {
        //    var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("Refresh_UsingMultipleCallsWithoutAsync_ShouldOnlyUpdateOnce");

        //    var resultsList = new ConcurrentBag<string>();

        //    Parallel.For(0, 3, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: false);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Assert.AreEqual<int>(2, resultsList.Count);
        //}

        //[TestMethod]
        //public void Refresh_UsingMultipleSubsequentCallsWithAsync_ShouldOnlyUpdateOnce() {
        //    var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("Refresh_UsingMultipleSubsequentCallsWithAsync_ShouldOnlyUpdateOnce");

        //    var resultsList = new ConcurrentBag<string>();

        //    Parallel.For(0, 3, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: true);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Parallel.For(0, 3, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: true);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Assert.AreEqual<int>(2, resultsList.Count);
        //}

        //[TestMethod]
        //public void Refresh_UsingMultipleSubsequentCallsWithoutAsync_ShouldOnlyUpdateOnce() {
        //    var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("Refresh_UsingMultipleSubsequentCallsWithoutAsync_ShouldOnlyUpdateOnce");

        //    var resultsList = new ConcurrentBag<string>();

        //    Parallel.For(0, 3, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: false);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Parallel.For(0, 3, (i) => {
        //        var blogService = GetRefreshTestBlogService(blogStore, refreshAsync: false);
        //        var refreshResults = blogService.Refresh();

        //        foreach(var result in refreshResults) {
        //            resultsList.Add(result);
        //        }
        //    });

        //    Assert.AreEqual<int>(2, resultsList.Count);
        //}

        private BlogService GetRefreshTestBlogService(IDocumentStore blogStore, bool refreshAsync, bool ensureBlogsRefreshed = false) {
            var blogKeys = new[] { "buzz", "status", };
            return BlogServiceTestHelper.GetBlogService(blogStore, blogKeys, refreshAsync, ensureBlogsRefreshed: ensureBlogsRefreshed);
        }
    }
}
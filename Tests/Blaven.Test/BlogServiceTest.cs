using System.Collections.Concurrent;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;
using Raven.Client.Embedded;

namespace Blaven.Test.Integration {
    [TestClass]
    public class BlogServiceTest {
        public BlogServiceTest() {
            
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

        //private BlogService GetRefreshTestBlogService(IDocumentStore blogStore, bool refreshAsync) {
        //    var blogKeys = new[] { "buzz", "status", };
        //    return BlogServiceTestHelper.GetBlogService(blogStore, blogKeys, refreshAsync, ensureBlogsRefreshed: false);
        //}
    }
}
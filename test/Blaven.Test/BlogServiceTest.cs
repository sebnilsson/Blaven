using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataSources;
using Blaven.RavenDb;
using Blaven.Test;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Client;

namespace Blaven.Integration.Test
{
    [TestClass]
    public class BlogServiceTest
    {
        private readonly IEnumerable<string> _blogKeys = new[] { "buzz_simple", "status_simple", };

        private readonly int _userCount = 3;

        private int _blogCount
        {
            get
            {
                return _blogKeys.Count();
            }
        }

        [TestMethod]
        public void ctor_WithEnsureBlogIsRefreshed_StoreShouldContainData()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            Parallel.For(0, 1, (i) => { var blogService = GetBlogServiceWithMultipleBlogs(documentStore); });

            var blogStore = new RavenRepository(documentStore);
            var blogsWithDataCount = _blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            var storeDocumentCount = documentStore.DatabaseCommands.GetStatistics().CountOfDocuments;

            Assert.AreEqual<int>(_blogCount, blogsWithDataCount, "The blogs did not have data at time of query.");
        }

        [TestMethod]
        public void ctor_WithoutEnsureBlogIsRefreshed_StoreShouldNotContainData()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            Parallel.For(
                0,
                _userCount,
                (i) => this.GetBlogServiceWithMultipleBlogs(documentStore, ensureBlogsRefreshed: false));

            var blogStore = new RavenRepository(documentStore);
            var blogsWithDataCount = _blogKeys.Count(x => blogStore.GetHasBlogAnyData(x));

            Assert.AreEqual<int>(0, blogsWithDataCount, "The blogs had data, when expected not to.");
        }

        [TestMethod]
        public void Refresh_WithEnsureBlogsRefreshed_FollowingRefreshesShouldNotUpdate()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);
            firstRunBlogService.Repository.WaitForStaleIndexes();

            var results = new ConcurrentBag<RefreshResult>();
            Parallel.For(
                0,
                _userCount,
                (i) =>
                    {
                        var blogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);

                        var updated = blogService.Refresh();
                        updated.ToList().ForEach(x => results.Add(x));
                    });

            var updatedCount =
                results.Count(x => x.ResultType == RefreshResultType.UpdateSync || x.ResultType == RefreshResultType.UpdateAsync);

            Assert.AreEqual(0, updatedCount, "The blogs were updated too many times.");
        }

        [TestMethod]
        public void Refresh_WithoutEnsureBlogsRefreshed_FollowingRefreshesShouldUpdateSynchronously()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(
                documentStore: documentStore, ensureBlogsRefreshed: false);
            firstRunBlogService.Repository.WaitForStaleIndexes();

            var results = new ConcurrentBag<RefreshResult>();
            Parallel.For(
                0,
                _userCount,
                (i) =>
                    {
                        var blogService = GetBlogServiceWithMultipleBlogs(
                            documentStore: documentStore, ensureBlogsRefreshed: false);

                        var userResults = blogService.Refresh();
                        userResults.ToList().ForEach(x => results.Add(x));
                    });

            var updatedCount = results.Count(x => x.ResultType == RefreshResultType.UpdateSync);

            Assert.AreEqual<int>(_blogCount, updatedCount, "The blogs weren't updated enough times.");
        }

        [TestMethod]
        public void Refresh_WithEnsureBlogsRefreshed_FollowingRefreshesWithForceRefreshShouldUpdateSynchronously()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);
            firstRunBlogService.Repository.WaitForStaleIndexes();

            var updated = firstRunBlogService.Refresh(forceRefresh: true);
            var updatedCount = updated.Count(x => x.ResultType == RefreshResultType.UpdateSync);

            Assert.AreEqual<int>(_blogKeys.Count(), updatedCount, "The blogs weren't updated again.");
        }

        [TestMethod]
        public void Refresh_WithoutZeroMinutesCaching_FollowingRefreshesShouldNotUpdateSynchronously()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var firstRunBlogService = GetBlogServiceWithMultipleBlogs(documentStore: documentStore);

            firstRunBlogService.Repository.WaitForStaleIndexes();

            var secondRefreshResults = new ConcurrentBag<RefreshResult>();
            Parallel.For(
                0,
                _userCount,
                (i) =>
                    {
                        var blogService = GetBlogServiceWithMultipleBlogs(
                            documentStore: documentStore, ensureBlogsRefreshed: false);
                        blogService.Config.CacheTime = 0;

                        var secondRefresh = blogService.Refresh();
                        foreach (var refresh in secondRefresh)
                        {
                            secondRefreshResults.Add(refresh);
                        }
                    });

            var updatedSynchronouslyCount = secondRefreshResults.Count(x => x.ResultType == RefreshResultType.UpdateSync);

            Assert.AreEqual<int>(0, updatedSynchronouslyCount, "The blogs were updated too many times.");
        }

        [TestMethod]
        public void Refresh_PostsWithUnencodedPreTagContent_ShouldNotCutContent()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var service = BlogServiceTestHelper.GetBlogService(
                documentStore, new[] { "jonasrapp" }, ensureBlogsRefreshed: true);
            service.Repository.WaitForStaleIndexes();

            var preTagPost = service.GetPostByDataSourceId(3351494039612406157);

            Assert.IsTrue(preTagPost.Content.Contains("</pre>"));
            Assert.IsTrue(preTagPost.Content.Contains("for (var i = 0; i &lt; regardingEntities.length; i++)"));
        }

        [TestMethod]
        public void Refresh_PostsWithPreTagContainingCodeTag_ShouldNotEncodeCodeTag()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var service = BlogServiceTestHelper.GetBlogService(
                documentStore, new[] { "jonasrapp" }, ensureBlogsRefreshed: true);
            service.Repository.WaitForStaleIndexes();

            var preTagPost = service.GetPostByDataSourceId(3351494039612406157);

            Assert.IsTrue(preTagPost.Content.Contains("<code>"));
            Assert.IsTrue(preTagPost.Content.Contains("</code>"));
        }

        [TestMethod]
        public void Refresh_PostsWithUnencodedPreTagContent_EncodesAllPreTags()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            var service = BlogServiceTestHelper.GetBlogService(
                documentStore, new[] { "jonasrapp" }, ensureBlogsRefreshed: true);
            service.Repository.WaitForStaleIndexes();

            var preTagPost = service.GetPostByDataSourceId(3776549820754361006);

            int preIndex = preTagPost.Content.IndexOf("<pre");
            int preCount = 0;
            while (preIndex >= 0)
            {
                preCount++;
                preIndex = preTagPost.Content.IndexOf("<pre", preIndex + 1);
            }

            Assert.AreEqual<int>(7, preCount, "Some instances of pre-tags disappeared.");
        }

        [TestMethod]
        public void Singleton_EnsureInit()
        {
            bool unInitHasNoPosts = true;
            bool isCorrectException = false;
            bool doesNotThrowExceptionAfterInit = true;
            bool initHasPosts = false;

            try
            {
                unInitHasNoPosts = !BlogService.Instance.GetPosts(0).Posts.Any();
            }
            catch (Exception ex)
            {
                isCorrectException = ex is BlavenNotInitException;
            }

            try
            {
                var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();
                var settings = BlogServiceTestHelper.GetBloggerSettings(_blogKeys.Skip(1));
                BlogService.InitInstance(documentStore, settings: settings);

                var postHeads = BlogService.Instance.GetAllPostHeads();
                initHasPosts = postHeads.Any();
            }
            catch (Exception)
            {
                doesNotThrowExceptionAfterInit = false;
            }

            Assert.IsTrue(isCorrectException, "No or incorrect exception thrown");
            Assert.IsTrue(doesNotThrowExceptionAfterInit, "Exception thrown, even though BlavenService is init");
            Assert.IsTrue(unInitHasNoPosts, "The uninit service has posts");
            Assert.IsTrue(initHasPosts, "The init service has no posts");
        }

        private BlogService GetBlogServiceWithMultipleBlogs(
            IDocumentStore documentStore = null,
            bool refreshAsync = true,
            bool ensureBlogsRefreshed = true,
            IEnumerable<string> blogKeys = null)
        {
            blogKeys = blogKeys ?? _blogKeys;
            documentStore = documentStore ?? DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            return BlogServiceTestHelper.GetBlogService(
                documentStore, blogKeys, refreshAsync, ensureBlogsRefreshed: ensureBlogsRefreshed);
        }
    }
}
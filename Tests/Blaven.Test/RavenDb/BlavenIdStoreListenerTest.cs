using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blaven.RavenDb;

namespace Blaven.Test.RavenDb {
    [TestClass]
    public class BlavenIdStoreListenerTest {
        private readonly int DefaultPageIndex = 0;
        private readonly int DefaultPageSize = 5;
        private readonly string _blogKey = "TEST";

        [TestMethod]
        public void ctor_WhenNewPostsAdded_ShouldAddIncrementalIds() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();
            documentStore.RegisterListener(new BlavenIdStoreListener(documentStore));

            BlogService.InitStore(documentStore);

            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 3));

            blogStore.Refresh(_blogKey, blogData, waitForIndexes: true);

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);
            var first = selection.Posts.First();
            var last = selection.Posts.Last();

            Assert.AreEqual<long>(1, first.BlavenId, "The BlavenId was not correctly set automatically.");
            Assert.AreEqual<long>(3, last.BlavenId, "The BlavenId was not correctly set automatically.");
        }

        [TestMethod]
        public void ctor_WhenAdditionalPostsAdded_ShouldAddIncrementalId() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();
            documentStore.RegisterListener(new BlavenIdStoreListener(documentStore));

            BlogService.InitStore(documentStore);

            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 3));

            blogStore.Refresh(_blogKey, blogData, waitForIndexes: true);

            var newBlogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 5));

            blogStore.Refresh(_blogKey, newBlogData, waitForIndexes: true);

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);
            var first = selection.Posts.First();
            var last = selection.Posts.Last();

            Assert.AreEqual<long>(1, first.BlavenId, "The BlavenId was not correctly set automatically.");
            Assert.AreEqual<long>(5, last.BlavenId, "The BlavenId was not correctly set automatically.");
        }
    }
}

using System.Linq;

using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test {
    [TestClass]
    public class RavenDbBlogStoreTest {
        private readonly string _blogKey = "TEST";

        [TestMethod]
        public void Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 2));

            blogStore.Refresh(_blogKey, blogData);
            DocumentStoreTestHelper.WaitForIndexes(documentStore);

            int totalPosts = blogStore.GetBlogSelection(0, 5, _blogKey).TotalPostsCount;
            Assert.AreEqual<int>(2, totalPosts, "The stored posts were not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 2));

            blogStore.Refresh(_blogKey, blogData);
            DocumentStoreTestHelper.WaitForIndexes(documentStore);

            blogData.Posts = blogData.Posts.Concat(BlogPostsTestHelper.GetBlogPosts(_blogKey, 3, 2));

            blogStore.Refresh(_blogKey, blogData);
            DocumentStoreTestHelper.WaitForIndexes(documentStore);

            var selection = blogStore.GetBlogSelection(0, 5, _blogKey);
            Assert.AreEqual<int>(4, selection.TotalPostsCount, "The added post was not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 2));

            blogStore.Refresh(_blogKey, blogData);
            DocumentStoreTestHelper.WaitForIndexes(documentStore);

            blogData.Posts = BlogPostsTestHelper.GetBlogPosts(_blogKey, 2);

            blogStore.Refresh(_blogKey, blogData);
            DocumentStoreTestHelper.WaitForIndexes(documentStore);

            var selection = blogStore.GetBlogSelection(0, 5, _blogKey);
            Assert.AreEqual<int>(2, selection.TotalPostsCount, "The removed posts was not removed from store.");
        }
        
        [TestMethod]
        public void GetBlogSelection_WhenContaining33Entries_ShouldContainTotal33Entries() {
            int postsCount = 33;
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("GetBlogSelection_WhenContaining33Entries_ShouldContainTotal33Entries");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, postsCount);
            
            var selection = blogStore.GetBlogSelection(0, 5);
            Assert.AreEqual<int>(postsCount, selection.TotalPostsCount, "The total amount of posts did not match the posts in the store.");
        }
    }
}

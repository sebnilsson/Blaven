using System;
using System.Linq;

using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test
{
    [TestClass]
    public class RavenRepositoryTest : BlavenTestBase
    {
        private const string DuplicatedItemTitlePrefix = "[DUPLICATE]";

        private const string UpdatedItemTitlePrefix = "[UPDATE]";

        [TestMethod]
        public void Refresh_WithDuplicateIdsAndThrowOnCritical_ShouldThrowException()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();
            var repository = new RavenRepository(documentStore);

            bool isErrorThrown = false;
            try
            {
                RefreshWithDuplicateIds(repository, throwOnCritical: true);
            }
            catch (BlavenException ex)
            {
                isErrorThrown = true;
            }

            Assert.IsTrue(isErrorThrown, "Expected error was not thrown");
        }

        [TestMethod]
        public void Refresh_WithDuplicateIdsAndWithoutThrowOnCritical_ShouldNotThrowException()
        {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();
            var repository = new RavenRepository(documentStore);

            bool isErrorThrown = false;
            bool isRepoDuplicatedItemUpdated = false;
            try
            {
                var duplicateItem = RefreshWithDuplicateIds(repository, throwOnCritical: false);

                var repoDuplicatedItem = repository.GetBlogPost(TestBlogKey, duplicateItem.Id);
                isRepoDuplicatedItemUpdated = repoDuplicatedItem.Title.StartsWith(UpdatedItemTitlePrefix);
            }
            catch (BlavenException ex)
            {
                isErrorThrown = true;
            }

            Assert.IsFalse(isErrorThrown, "Unexpected error was thrown");
            Assert.IsTrue(isRepoDuplicatedItemUpdated, "Duplicated item has been updated unexpectedly");
        }

        private BlogPost RefreshWithDuplicateIds(RavenRepository repository, bool throwOnCritical)
        {
            var posts = BlogPostsTestHelper.GetBlogPosts(TestBlogKey, 3).ToList();
            var blogData = new BlogData { Info = new BlogInfo(), Posts = posts };

            repository.Refresh(TestBlogKey, blogData, throwOnCritical);
            repository.WaitForStaleIndexes();

            var updatedItem = posts[0];
            var duplicateItem = posts[1];

            updatedItem.Title = UpdatedItemTitlePrefix + updatedItem.Title;

            duplicateItem.BlavenId = updatedItem.BlavenId;
            duplicateItem.Id = updatedItem.Id;
            duplicateItem.Title = DuplicatedItemTitlePrefix + duplicateItem.Title;

            repository.Refresh(TestBlogKey, blogData, throwOnCritical);
            repository.WaitForStaleIndexes();

            return duplicateItem;
        }
    }
}
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test
{
    [TestClass]
    public class RepositoryRefreshHelperTest : RepositoryTestBase
    {
        private const string DuplicatedItemTitlePrefix = "[DUPLICATE]";

        private const string UpdatedItemTitlePrefix = "[UPDATE]";

        [TestMethod]
        public void Refresh_WithDuplicateIdsAndThrowOnCritical_ShouldThrowException()
        {
            var repository = GetRepository();

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
            var repository = GetRepository();

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

        private static BlogPost RefreshWithDuplicateIds(Repository repository, bool throwOnCritical)
        {
            var posts = GenerateBlogPosts(3).ToList();
            var blogData = GenerateBlogData(posts);

            repository.Refresh(TestBlogKey, blogData, throwOnCritical);
            repository.WaitForPosts();

            var updatedItem = posts[0];
            var duplicateItem = posts[1];

            updatedItem.Title = UpdatedItemTitlePrefix + updatedItem.Title;

            duplicateItem.BlavenId = updatedItem.BlavenId;
            duplicateItem.Id = updatedItem.Id;
            duplicateItem.Title = DuplicatedItemTitlePrefix + duplicateItem.Title;

            repository.Refresh(TestBlogKey, blogData, throwOnCritical);
            repository.WaitForPosts();

            return duplicateItem;
        }
    }
}
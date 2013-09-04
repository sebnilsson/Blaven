using Blaven.RavenDb;
using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.DataSources.Blogger.Test
{
    [TestClass]
    public class BloggerDataSourceTest
    {
        private const string TestBlogKey = "TEST";

        [TestMethod]
        public void DataSourceUpdated_AddPost_ShouldExistAfterAdd()
        {
            ulong addedItemId = 5683021850220643338;
            var blogger = new BloggerDataSource();
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            string initialDataSourceUri = TestEnvironmentHelper.GetXmlFilePath("buzz_simple.xml");
            var initialSetting = new BlavenBlogSetting(TestBlogKey, blogger) { DataSourceUri = initialDataSourceUri };
            var initialService = new BlogService(documentStore, settings: new[] { initialSetting });

            bool addedPostExistsBeforeAdd = initialService.GetPostByDataSourceId(addedItemId) != null;

            string updatedDataSourceUri = TestEnvironmentHelper.GetXmlFilePath("buzz_simple_added.xml");
            var updatedSetting = new BlavenBlogSetting(TestBlogKey, blogger) { DataSourceUri = updatedDataSourceUri };
            var updatedService = new BlogService(documentStore, settings: new[] { updatedSetting });
            updatedService.Refresh(forceRefresh: true);

            bool addedPostExistsAfterAdd = updatedService.GetPostByDataSourceId(addedItemId) != null;

            Assert.IsFalse(addedPostExistsBeforeAdd, "Added post exists before update");
            Assert.IsTrue(addedPostExistsAfterAdd, "Added post doesn't exist after add");
        }

        [TestMethod]
        public void DataSourceUpdated_RemovePost_ShouldBeFlaggedDeletedAfterRemove()
        {
            ulong removedItemId = 6427494462457519921;
            var blogger = new BloggerDataSource();
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore();

            string initialDataSourceUri = TestEnvironmentHelper.GetXmlFilePath("buzz_simple.xml");
            var initialSetting = new BlavenBlogSetting(TestBlogKey, blogger) { DataSourceUri = initialDataSourceUri };
            var initialService = new BlogService(documentStore, settings: new[] { initialSetting });
            
            bool removedPostExistsBeforeRemove = initialService.GetPostByDataSourceId(removedItemId) != null;

            string updatedDataSourceUri = TestEnvironmentHelper.GetXmlFilePath("buzz_simple_removed.xml");
            var updatedSetting = new BlavenBlogSetting(TestBlogKey, blogger) { DataSourceUri = updatedDataSourceUri };
            var updatedService = new BlogService(documentStore, settings: new[] { updatedSetting });
            updatedService.Refresh(forceRefresh: true);

            var removedPost = updatedService.GetPostByDataSourceId(removedItemId);
            bool removedPostIsFlaggedAsRemoved = removedPost != null && removedPost.IsDeleted;

            Assert.IsTrue(removedPostExistsBeforeRemove, "Removed post doesn't exist before remove");
            Assert.IsNotNull(removedPost, "Removed post is deleted, not flagged as deleted");
            Assert.IsTrue(removedPostIsFlaggedAsRemoved, "Removed post isn't flagged as removed");
        }
    }
}
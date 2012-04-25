using Microsoft.VisualStudio.TestTools.UnitTesting;

using Raven.Client.Embedded;

namespace BloggerViewController.Test {
    [TestClass]
    public class BlogServiceTest {
        public BlogServiceTest() {
            
        }
        
        private static EmbeddableDocumentStore BaseDocumentStore { get; set; }

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context) {
            BaseDocumentStore = TestHelper.GetEmbeddableDocumentStore("BlogServiceTest");
        }

        [ClassCleanup()]
        public static void ClassCleanup() {
        }

        [TestMethod]
        public void GetArchiveCount_ShouldReturnRightAmountOfArchiveKeys() {
            // Arrange
            var blogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz");

            // Act
            var archiveCount = blogService.GetArchiveCount();
            
            // Assert
            Assert.AreEqual<int>(archiveCount.Keys.Count, 86);
        }

        [TestMethod]
        public void GetTagsCount_ShouldReturnRightAmountOfTagKeys() {
            // Arrange
            var blogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz");

            // Act
            var tagCount = blogService.GetTagsCount();

            // Assert
            Assert.AreEqual<int>(tagCount.Keys.Count, 77);
        }
    }
}
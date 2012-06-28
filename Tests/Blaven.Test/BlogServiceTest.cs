using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Raven.Client.Embedded;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Blaven.Test.Integration {
    [TestClass]
    public class BlogServiceTest {
        public BlogServiceTest() {
            
        }
        
        private static EmbeddableDocumentStore BaseDocumentStore { get; set; }

        [ClassInitialize()]
        public static void ClassInitialize(TestContext context) {
            BaseDocumentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("BlogServiceTest");
        }

        [ClassCleanup()]
        public static void ClassCleanup() {

        }

        [TestMethod]
        public void ctor_UsingMultipleCalls_ShouldOnlyUpdateOnce() {
            var blogStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("ctor_UsingMultipleCalls_ShouldOnlyUpdateOnce");

            var resultsList = new ConcurrentDictionary<string, int>();

            Parallel.For(0, 3, (i) => {
                string blogKey = "buzz";
                var blogService = BlogServiceTestHelper.GetBlogService(blogStore, blogKey, "buzz");
                var info = blogService.GetInfo(blogKey);
            });
            
            //Assert.AreEqual<int>(1, trueCount);
        }

        //[TestMethod]
        //public void GetSelection_ShouldReturnCorrectCountAfterPostAdded() {
        //    // Arrange
        //    var beforeUpdateBlogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz", "buzz_simple");

        //    // Act
        //    var selectionBeforeUpdate = beforeUpdateBlogService.GetSelection(0);
        //    int countBeforeUpdate = selectionBeforeUpdate.TotalPostsCount;
            
        //    var afterUpdateBlogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz", "buzz_simple_added");
        //    var selectionAfterUpdate = afterUpdateBlogService.GetSelection(0);
        //    int countAfterUpdate = selectionAfterUpdate.TotalPostsCount;

        //    // Assert
        //    Assert.AreEqual<int>(countBeforeUpdate + 2, countAfterUpdate);
        //}

        //[TestMethod]
        //public void GetSelection_ShouldReturnCorrectCountAfterPostRemoved() {
        //    // Arrange
        //    var beforeUpdateBlogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz", "buzz_simple");

        //    // Act
        //    var selectionBeforeUpdate = beforeUpdateBlogService.GetSelection(0);
        //    int countBeforeUpdate = selectionBeforeUpdate.TotalPostsCount;

        //    var afterUpdateBlogService = TestHelper.GetBlogService(BaseDocumentStore, "buzz", "buzz_simple_removed");
        //    var selectionAfterUpdate = afterUpdateBlogService.GetSelection(0);
        //    int countAfterUpdate = selectionAfterUpdate.TotalPostsCount;

        //    // Assert
        //    Assert.AreEqual<int>(countBeforeUpdate - 2, countAfterUpdate);
        //}
    }
}
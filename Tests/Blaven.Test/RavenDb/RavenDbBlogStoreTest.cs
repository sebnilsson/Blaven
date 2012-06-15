using System.Linq;

using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test {
    [TestClass]
    public class RavenDbBlogStoreTest {
        [TestMethod]
        public void Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts() {
            var documentStore = TestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            string blogKey = "test";
            var blogData = new BlogData {
                Info = new BlogInfo {
                    BlogKey = blogKey,
                    Title = "TEST_TITLE",
                },
                Posts = new[] {
                    new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("1")),
                    new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("2")),
                }
            };

            blogStore.Refresh(blogKey, blogData);

            TestHelper.WaitForIndexes(documentStore);

            int totalPosts = blogStore.GetBlogSelection(0, 5, blogKey).TotalPostsCount;
            Assert.AreEqual<int>(2, totalPosts);
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts() {
            var documentStore = TestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            string blogKey = "test";
            var blogData = new BlogData {
                Info = new BlogInfo {
                    BlogKey = blogKey,
                    Title = "TEST_TITLE",
                },
                Posts = new [] {
                    new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("1")),
                    new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("2")),
                }
            };

            blogStore.Refresh(blogKey, blogData);

            TestHelper.WaitForIndexes(documentStore);

            blogData.Posts = blogData.Posts.Concat(new[] {
                new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("3")),
                new BlogPost(blogKey, RavenDbBlogStore.GetKey<BlogPost>("4")),
            });

            blogStore.Refresh(blogKey, blogData);
            
            TestHelper.WaitForIndexes(documentStore);

            var selection = blogStore.GetBlogSelection(0, 5, blogKey);
            Assert.AreEqual<int>(4, selection.TotalPostsCount);
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts() {
            var documentStore = TestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            string blogKey = "test";
            var blogData = new BlogData {
                Info = new BlogInfo {
                    BlogKey = blogKey,
                    Title = "TEST_TITLE",
                },
                Posts = new[] {
                    new BlogPost(blogKey, "1"),
                    new BlogPost(blogKey, "2"),
                    new BlogPost(blogKey, "3"),
                    new BlogPost(blogKey, "4"),
                }
            };

            blogStore.Refresh(blogKey, blogData);

            TestHelper.WaitForIndexes(documentStore);

            blogData.Posts = new[] {
                new BlogPost(blogKey, "1"),
                new BlogPost(blogKey, "2"),
            };

            blogStore.Refresh(blogKey, blogData);

            TestHelper.WaitForIndexes(documentStore);
            
            var selection = blogStore.GetBlogSelection(0, 5, blogKey);
            Assert.AreEqual<int>(2, selection.TotalPostsCount);
        }
    }
}

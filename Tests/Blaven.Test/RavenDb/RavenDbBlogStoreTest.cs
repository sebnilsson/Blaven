using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test {
    [TestClass]
    public class RavenDbBlogStoreTest {
        private readonly int DefaultPageIndex = 0;
        private readonly int DefaultPageSize = 5;
        private readonly string _blogKey = "TEST";

        [TestMethod]
        public void Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 2));

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);
            int totalPosts = selection.TotalPostCount;

            Assert.AreEqual<int>(2, totalPosts, "The stored posts were not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 2));

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            blogData.Posts = blogData.Posts.Concat(BlogPostsTestHelper.GetBlogPosts(_blogKey, 3, 2));

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);

            Assert.AreEqual<int>(4, selection.TotalPostCount, "The added post was not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("RavenDbBlogStoreTest.Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, BlogPostsTestHelper.GetBlogPosts(_blogKey, 4));

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            blogData.Posts = BlogPostsTestHelper.GetBlogPosts(_blogKey, 2);

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);

            Assert.AreEqual<int>(2, selection.TotalPostCount, "The removed posts was not removed from store.");
        }
        
        [TestMethod]
        public void GetBlogSelection_WhenContaining33Entries_ShouldContainTotal33Entries() {
            int postsCount = 33;
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("GetBlogSelection_WhenContaining33Entries_ShouldContainTotal33Entries");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, postsCount);

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var selection = blogStore.GetBlogSelection(0, 5, _blogKey);

            Assert.AreEqual<int>(postsCount, selection.TotalPostCount, "The total amount of posts did not match the posts in the store.");
        }

        [TestMethod]
        public void GetBlogInfo_WhenChangeInBlogInfo_ShouldShowChanges() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("GetBlogInfo_WhenChangeInBlogInfo_ShouldShowChanges");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, 1);
            blogData.Info.Subtitle = "ORIGINAL_SUBTITLE";
            blogData.Info.Title = "ORIGINAL_TITLE";
            blogData.Info.Updated = DateTime.MinValue;
            blogData.Info.Url = "ORIGINAL_URL";

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            string updatedSubtitle = "UPDATED_SUBTITLE";
            string updatedTitle = "UPDATED_TITLE";
            DateTime updatedUpdated = DateTime.MinValue.AddDays(1);
            string updatedUrl = "UPDATED_URL";

            blogData.Info.Subtitle = updatedSubtitle;
            blogData.Info.Title = updatedTitle;
            blogData.Info.Updated = updatedUpdated;
            blogData.Info.Url = updatedUrl;

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var info = blogStore.GetBlogInfo(_blogKey);

            Assert.AreEqual<string>(updatedSubtitle, info.Subtitle, "Subtitle wasn't updated in store.");
            Assert.AreEqual<string>(updatedTitle, info.Title, "Title wasn't updated in store.");
            Assert.AreEqual<DateTime>(updatedUpdated, info.Updated, "Updated wasn't updated in store.");
            Assert.AreEqual<string>(updatedUrl, info.Url, "Url wasn't updated in store.");
        }

        [TestMethod]
        public void GetBlogSelection_WhenChangeInBlogPost_ShouldShowChanges() {
            var documentStore = DocumentStoreTestHelper.GetEmbeddableDocumentStore("GetBlogSelection_WhenChangeInBlogPost_ShouldShowChanges");
            var blogStore = new RavenDbBlogStore(documentStore);
            var blogData = BlogDataTestHelper.GetBlogData(_blogKey, 2);
            blogData.Posts = blogData.Posts.ToList();
            
            var post = blogData.Posts.Last(); // Using second post to avoid false positives
            post.Author.ImageUrl = "ORIGINAL_AUTHOR_IMAGE_URL";
            post.Author.Name = "ORIGINAL_AUTHOR_NAME";
            post.Content = "ORIGINAL_CONTENT";
            post.PermaLinkAbsolute = "ORIGINAL_PERMALINK_ABSOLUTE";
            post.PermaLinkRelative = "ORIGINAL_PERMALINK_RELATIVE";
            post.Published = DateTime.MinValue;
            post.Tags = new [] { "TAG_1", "TAG_2" };
            post.Title = "ORIGINAL_TITLE";
            post.Updated = DateTime.MinValue.AddYears(1);

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            string updatedAuthorImageUrl = "UPDATED_AUTHOR_IMAGE_URL";
            string updatedAuthorName = "UPDATED_AUTHOR_NAME";
            string updatedContent = "UPDATED_CONTENT";
            string updatedPermaLinkAbsolute = "UPDATED_PERMALINK_ABSOLUTE";
            string updatedPermaLinkRelative = "UPDATED_PERMALINK_RELATIVE";
            DateTime updatedPublished = DateTime.MinValue.AddDays(1);
            IEnumerable<string> updatedTags = new [] { "TAG_1", "TAG_2", "TAG_3" };
            string updatedTitle = "UPDATED_TITLE";
            DateTime updatedUpdated = DateTime.MinValue.AddYears(1).AddDays(1);

            post.Author.ImageUrl = updatedAuthorImageUrl;
            post.Author.Name = updatedAuthorName;
            post.Content = updatedContent;
            post.PermaLinkAbsolute = updatedPermaLinkAbsolute;
            post.PermaLinkRelative = updatedPermaLinkRelative;
            post.Published = updatedPublished;
            post.Tags = updatedTags;
            post.Title = updatedTitle;
            post.Updated = updatedUpdated;

            blogStore.Refresh(_blogKey, blogData);
            blogStore.WaitForIndexes();

            var selection = blogStore.GetBlogSelection(DefaultPageIndex, DefaultPageSize, _blogKey);
            var selectedPost = selection.Posts.First(x => x.Id == post.Id);

            Assert.AreEqual<string>(updatedAuthorImageUrl, selectedPost.Author.ImageUrl, "AuthorImageUrl wasn't updated in store.");
            Assert.AreEqual<string>(updatedAuthorName, selectedPost.Author.Name, "AuthorName wasn't updated in store.");
            Assert.AreEqual<string>(updatedContent, selectedPost.Content, "Content wasn't updated in store.");
            Assert.AreEqual<string>(updatedPermaLinkAbsolute, selectedPost.PermaLinkAbsolute, "PermaLinkAbsolute wasn't updated in store.");
            Assert.AreEqual<string>(updatedPermaLinkRelative, selectedPost.PermaLinkRelative, "PermaLinkRelative wasn't updated in store.");
            Assert.AreEqual<DateTime>(updatedPublished, selectedPost.Published, "Published wasn't updated in store.");
            Assert.AreEqual<int>(updatedTags.Count(), selectedPost.Tags.Count(), "Tags wasn't updated in store.");
            Assert.AreEqual<string>(updatedTitle, selectedPost.Title, "Title wasn't updated in store.");
            Assert.AreEqual<DateTime>(updatedUpdated, selectedPost.Updated, "Updated wasn't updated in store.");
        }
    }
}

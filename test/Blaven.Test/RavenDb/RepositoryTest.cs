using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.DataSources;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.RavenDb.Test
{
    [TestClass]
    public class RepositoryTest : RepositoryTestBase
    {
        private const int DefaultPageIndex = 0;

        private const int DefaultPageSize = 5;

        [TestMethod]
        public void Resfresh_WhenBlogPostAdded_ShouldContainAddedPosts()
        {
            var repository = GetRepository();

            var blogData = GenerateBlogData(2);

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            var selection = repository.GetBlogSelection(DefaultPageIndex, DefaultPageSize, TestBlogKey);
            int totalPosts = selection.TotalPostCount;

            Assert.AreEqual<int>(2, totalPosts, "The stored posts were not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostAddedAndSecondRefresh_ShouldContainAddedPosts()
        {
            var repository = GetRepository();

            var blogData = GenerateBlogData(2);

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            blogData.Posts = blogData.Posts.Concat(GenerateBlogPosts(3, 2));

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            var selection = repository.GetBlogSelection(DefaultPageIndex, DefaultPageSize, TestBlogKey);

            Assert.AreEqual<int>(4, selection.TotalPostCount, "The added post was not added to store.");
        }

        [TestMethod]
        public void Resfresh_WhenBlogPostsRemovedAndSecondRefresh_ShouldNotContainRemovedPosts()
        {
            var repository = GetRepository();
            
            var blogInfo = new BlogInfo { BlogKey = TestBlogKey, Title = "TEST_TITLE" };
            var refreshResultBefore = new DataSourceRefreshResult
                                          {
                                              BlogInfo = blogInfo,
                                              ModifiedBlogPosts = GenerateBlogPosts(4),
                                              RemovedBlogPostIds = Enumerable.Empty<string>()
                                          };
            repository.Refresh(TestBlogKey, refreshResultBefore);
            repository.WaitForPosts();

            var removedBlogs = refreshResultBefore.ModifiedBlogPosts.Skip(2).Select(x => x.Id);
            var refreshResultAfter = new DataSourceRefreshResult
                                         {
                                             BlogInfo = blogInfo,
                                             ModifiedBlogPosts = Enumerable.Empty<BlogPost>(),
                                             RemovedBlogPostIds = removedBlogs
                                         };
            repository.Refresh(TestBlogKey, refreshResultAfter);
            repository.WaitForPosts();

            var selection = repository.GetBlogSelection(DefaultPageIndex, DefaultPageSize, TestBlogKey);

            Assert.AreEqual<int>(2, selection.TotalPostCount, "The removed posts was not removed from store.");
        }

        private IEnumerable<BlogPost> GetBlogTagsSelectionBlogPosts()
        {
            return new[]
                       {
                           new BlogPost("BLOGKEY1", 1) { Tags = new[] { "TESTTAG", } },
                           new BlogPost("BLOGKEY1", 2) { Tags = new[] { "TESTTAG", } },
                           new BlogPost("BLOGKEY2", 3) { Tags = new[] { "TESTTAG", } },
                       };
        }

        private Repository GetBlogTagsSelectionRepository()
        {
            var repository = GetRepository();

            string blogKey1 = "BLOGKEY1";
            var blogData1 = new BlogData { Posts = GetBlogTagsSelectionBlogPosts().Where(x => x.BlogKey == blogKey1), };
            repository.Refresh(blogKey1, blogData1);

            string blogKey2 = "BLOGKEY2";
            var blogData2 = new BlogData { Posts = GetBlogTagsSelectionBlogPosts().Where(x => x.BlogKey == blogKey2), };
            repository.Refresh(blogKey2, blogData2);
            repository.WaitForPosts();

            return repository;
        }

        [TestMethod]
        public void GetBlogTagsSelection_WhenSelectingAllBlogs_ShouldReturnAllBlogsTags()
        {
            var repository = GetBlogTagsSelectionRepository();

            var tagSelection = repository.GetBlogTagsSelection(
                "TESTTAG", 0, int.MaxValue, new[] { "BLOGKEY1", "BLOGKEY2", });

            Assert.AreEqual<int>(3, tagSelection.TotalPostCount);
        }

        [TestMethod]
        public void GetBlogTagsSelection_WhenSelectingOneBlogs_ShouldReturnOneBlogsTags()
        {
            var repository = GetBlogTagsSelectionRepository();

            var tagSelection = repository.GetBlogTagsSelection("TESTTAG", 0, int.MaxValue, new[] { "BLOGKEY2", });

            Assert.AreEqual<int>(1, tagSelection.TotalPostCount);
        }

        [TestMethod]
        public void GetBlogSelection_WhenContaining33Entries_ShouldContainTotalCorrectAmountOfEntries()
        {
            var repository = GetRepository();

            int postsCount = 33;

            var blogData = GenerateBlogData(postsCount);

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            var selection = repository.GetBlogSelection(0, 5, TestBlogKey);

            Assert.AreEqual<int>(
                postsCount, selection.TotalPostCount, "The total amount of posts did not match the posts in the store.");
        }

        [TestMethod]
        public void GetBlogInfo_WhenChangeInBlogInfo_ShouldShowChanges()
        {
            var repository = GetRepository();

            var blogData = GenerateBlogData(1);
            blogData.Info.Subtitle = "ORIGINAL_SUBTITLE";
            blogData.Info.Title = "ORIGINAL_TITLE";
            blogData.Info.Updated = DateTime.MinValue;
            blogData.Info.Url = "ORIGINAL_URL";

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            string updatedSubtitle = "UPDATED_SUBTITLE";
            string updatedTitle = "UPDATED_TITLE";
            DateTime updatedUpdated = DateTime.MinValue.AddDays(1);
            string updatedUrl = "UPDATED_URL";

            blogData.Info.Subtitle = updatedSubtitle;
            blogData.Info.Title = updatedTitle;
            blogData.Info.Updated = updatedUpdated;
            blogData.Info.Url = updatedUrl;

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            var info = repository.GetBlogInfo(TestBlogKey);

            Assert.AreEqual<string>(updatedSubtitle, info.Subtitle, "Subtitle wasn't updated in store.");
            Assert.AreEqual<string>(updatedTitle, info.Title, "Title wasn't updated in store.");
            Assert.AreEqual<DateTime?>(updatedUpdated, info.Updated, "Updated wasn't updated in store.");
            Assert.AreEqual<string>(updatedUrl, info.Url, "Url wasn't updated in store.");
        }

        [TestMethod]
        public void GetBlogSelection_WhenChangeInBlogPost_ShouldShowChanges()
        {
            var repository = GetRepository();

            var blogData = GenerateBlogData(2);
            blogData.Posts = blogData.Posts.ToList();

            var post = blogData.Posts.Last(); // Using second post to avoid false positives
            post.Author.ImageUrl = "ORIGINAL_AUTHOR_IMAGE_URL";
            post.Author.Name = "ORIGINAL_AUTHOR_NAME";
            post.Content = "ORIGINAL_CONTENT";
            post.DataSourceUrl = "ORIGINAL_ORIGINAL_BLOGGER_URL";
            post.Published = DateTime.MinValue;
            post.Tags = new[] { "TAG_1", "TAG_2" };
            post.Title = "ORIGINAL_TITLE";
            post.Updated = DateTime.MinValue.AddYears(1);
            post.UrlSlug = "ORIGINAL_URL_SLUG";

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            string updatedAuthorImageUrl = "UPDATED_AUTHOR_IMAGE_URL";
            string updatedAuthorName = "UPDATED_AUTHOR_NAME";
            string updatedContent = "UPDATED_CONTENT";
            string updatedOriginalBloggerUrl = "UPDATED_ORIGINAL_BLOGGER_URL";
            DateTime updatedPublished = DateTime.MinValue.AddDays(1);
            IEnumerable<string> updatedTags = new[] { "TAG_1", "TAG_2", "TAG_3" };
            string updatedTitle = "UPDATED_TITLE";
            DateTime updatedUpdated = DateTime.MinValue.AddYears(1).AddDays(1);
            string updatedUrlSlug = "UPDATED_URL_SLUG";

            post.Author.ImageUrl = updatedAuthorImageUrl;
            post.Author.Name = updatedAuthorName;
            post.Content = updatedContent;
            post.DataSourceUrl = updatedOriginalBloggerUrl;
            post.Published = updatedPublished;
            post.Tags = updatedTags;
            post.Title = updatedTitle;
            post.Updated = updatedUpdated;
            post.UrlSlug = updatedUrlSlug;

            repository.Refresh(TestBlogKey, blogData);
            repository.WaitForPosts();

            var selection = repository.GetBlogSelection(DefaultPageIndex, DefaultPageSize, TestBlogKey);
            var selectedPost = selection.Posts.First(x => x.Id == post.Id);

            Assert.AreEqual<string>(
                updatedAuthorImageUrl, selectedPost.Author.ImageUrl, "AuthorImageUrl wasn't updated in store.");
            Assert.AreEqual<string>(updatedAuthorName, selectedPost.Author.Name, "AuthorName wasn't updated in store.");
            Assert.AreEqual<string>(updatedContent, selectedPost.Content, "Content wasn't updated in store.");
            Assert.AreEqual<string>(
                updatedOriginalBloggerUrl, selectedPost.DataSourceUrl, "OriginalBloggerUrl wasn't updated in store.");
            Assert.AreEqual<DateTime>(updatedPublished, selectedPost.Published, "Published wasn't updated in store.");
            Assert.AreEqual<int>(updatedTags.Count(), selectedPost.Tags.Count(), "Tags wasn't updated in store.");
            Assert.AreEqual<string>(updatedTitle, selectedPost.Title, "Title wasn't updated in store.");
            Assert.AreEqual<DateTime?>(updatedUpdated, selectedPost.Updated, "Updated wasn't updated in store.");
            Assert.AreEqual<string>(updatedUrlSlug, selectedPost.UrlSlug, "UrlSlug wasn't updated in store.");
        }

        private static Repository GetRepository()
        {
            var documentStore = GetEmbeddableDocumentStore();
            return new Repository(documentStore);
        }
    }
}
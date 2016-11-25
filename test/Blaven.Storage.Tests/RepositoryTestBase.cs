using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Tests;
using Xunit;

namespace Blaven.DataStorage.Tests
{
    public abstract class RepositoryTestBase
    {
        public virtual void GetAllBlogMetas_ReturnsBlogMetasWithAllFieldValues()
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = this.GetRepository(blogMetas: dbBlogMetas);

            // Act
            var blogMetasQuery = repository.GetBlogMetas();

            var blogMetas = blogMetasQuery.Skip(0).Take(100).ToList();

            // Assert
            bool hasBlogMetasAllFieldValues =
                blogMetas.All(
                    x =>
                        x.BlogKey != null && x.Description != null && x.Name != null && x.PublishedAt != null
                        && x.SourceId != null && x.UpdatedAt != null && x.Url != null);

            Assert.True(hasBlogMetasAllFieldValues);
        }

        public async Task GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta(string blogKey)
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = this.GetRepository(blogMetas: dbBlogMetas);

            // Act
            var blogMeta = await repository.GetBlogMeta(blogKey);

            // Assert
            Assert.Equal(blogKey, blogMeta.BlogKey);
        }

        public async Task GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = this.GetRepository(blogMetas: dbBlogMetas);

            // Act
            var blogMeta = await repository.GetBlogMeta("NON_EXISTING_BLOG_KEY");

            // Assert
            Assert.Null(blogMeta);
        }

        public async Task GetPost_ExistingBlavenId_ReturnsPost(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost(BlogMetaTestData.BlogKey, BlogPostTestData.BlavenId2);

            // Assert
            bool hasBlogPostFieldValues = HasBlogPostAllFieldValues(post);

            Assert.Equal(BlogMetaTestData.BlogKey, post.BlogKey);
            Assert.Equal(BlogPostTestData.BlavenId2, post.BlavenId);
            Assert.True(hasBlogPostFieldValues);
        }

        public async Task GetPost_ExistingBlavenIdAndNonExistingBlogKey_ReturnsNull(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost("NON_EXISTING_BLOG_KEY", BlogPostTestData.BlavenId2);

            // Assert
            Assert.Null(post);
        }

        public async Task GetPost_NonExistingBlavenId_ReturnsNull(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost(BlogMetaTestData.BlogKey, "NON_EXISTING_BLAVEN_ID");

            // Assert
            Assert.Null(post);
        }

        public async Task GetPostBySourceId_ExistingBlogKeyAndNotExistingSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, "NON_EXISTING_SOURCE_ID");

            // Assert
            Assert.Null(post);
        }

        public async Task GetPostBySourceId_ExistingBlogKeyAndSourceId_ReturnsBlogPost(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string sourceId = BlogPostTestData.CreatePostSourceId(0);

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, sourceId);

            // Assert
            Assert.Equal(sourceId, post.SourceId);
        }

        public async Task GetPostBySourceId_ExistingBlogKeyAndUpperCaseSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string upperCaseSourceId = BlogPostTestData.CreatePostSourceId(0).ToUpperInvariant();

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, upperCaseSourceId);

            // Assert
            Assert.Null(post);
        }

        public async Task GetPostBySourceId_ExistingSourceIdAndNonExistingBlogKey_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string sourceId = BlogPostTestData.CreatePostSourceId(0);

            // Act
            var post = await repository.GetPostBySourceId("NON_EXISTING_BLOG_KEY", sourceId);

            // Assert
            Assert.Null(post);
        }

        public virtual void ListArchive_ExistingBlogKey_ReturnsArchive(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var archiveQuery = repository.ListArchive(new[] { BlogMetaTestData.BlogKey });

            var archive = archiveQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allArchiveItemsMatchBlogKey = archive.Any() && archive.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allTagItemsHasDate = archive.All(x => x.Date > DateTime.MinValue);
            bool allTagItemsHasCount = archive.All(x => x.Count > 0);

            Assert.True(allArchiveItemsMatchBlogKey);
            Assert.True(allTagItemsHasDate);
            Assert.True(allTagItemsHasCount);
        }

        public virtual void ListArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var archive = repository.ListArchive(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();
            Assert.False(anyArchiveItems);
        }

        public virtual void ListTags_ExistingBlogKey_ReturnsListTags(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var tagsQuery = repository.ListTags(new[] { BlogMetaTestData.BlogKey });

            var tags = tagsQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allTagItemsMatchBlogKey = tags.Any() && tags.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allTagItemsHasNames = tags.All(x => !string.IsNullOrWhiteSpace(x.Name));
            bool allTagItemsHasCount = tags.All(x => x.Count > 0);

            Assert.True(allTagItemsMatchBlogKey);
            Assert.True(allTagItemsHasNames);
            Assert.True(allTagItemsHasCount);
        }

        public virtual void ListTags_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var tags = repository.ListTags(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyTagItems = tags.Any();

            Assert.False(anyTagItems);
        }

        public virtual void ListTags_DifferentCasingTags_ReturnsGroupedTags()
        {
            // Arrange
            var tagNames = new[] { "Test tag", "test tag", "TEST tag", "test TAG", "TEST TAG" };
            var dbBlogPosts =
                tagNames.Select(
                    (x, i) =>
                        new BlogPost
                            {
                                BlogKey = (i < 2) ? BlogMetaTestData.BlogKey : BlogMetaTestData.BlogKey.ToLowerInvariant(),
                                BlavenId = BlogPostTestData.CreateBlavenId(i),
                                PublishedAt = BlogPostTestData.TestPublishedAt,
                                BlogPostTags = new List<BlogPostTag> { new BlogPostTag(x) }
                            }).ToList();

            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var tags = repository.ListTags(new[] { BlogMetaTestData.BlogKey }).ToList();

            // Assert
            var testTagItem = tags.FirstOrDefault(x => x.Name == "Test tag");

            Assert.NotNull(testTagItem);
            Assert.Equal(5, testTagItem.Count);
        }

        public virtual void ListPostHeads_ExistingBlogKey_ReturnsPostHeads(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var postHeadsQuery = repository.ListPostHeads(new[] { BlogMetaTestData.BlogKey });

            var postHeads = postHeadsQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostHeadsMatchBlogKey = postHeads.Any() && postHeads.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = postHeads.All(HasBlogPostHeadAllFieldValues);

            Assert.True(allPostHeadsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
        }

        public virtual void ListPostHeads_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var postHeads = repository.ListPostHeads(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyPostHeads = postHeads.Any();
            Assert.False(anyPostHeads);
        }

        public virtual void ListPosts_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var postQuery = repository.ListPosts(new[] { BlogMetaTestData.BlogKey });

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
        }

        public virtual void ListPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var posts = repository.ListPosts(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyArchiveItems = posts.Any();
            Assert.False(anyArchiveItems);
        }

        public virtual void ListPostsByArchive_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var archiveDate = BlogPostTestData.TestPublishedAt;

            // Act
            var postQuery = repository.ListPostsByArchive(new[] { BlogMetaTestData.BlogKey }, archiveDate);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsMatchDate =
                posts.All(
                    x => x.PublishedAt?.Year == archiveDate.Year && x.PublishedAt.Value.Month == archiveDate.Month);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsMatchDate);
        }

        public virtual void ListPostsByArchive_ExistingBlogKeyAndNonExistingArchiveDate_ReturnsEmpty(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var nonExistingArchiveDate = BlogPostTestData.TestPublishedAt.AddYears(-1);

            // Act
            var archive =
                repository.ListPostsByArchive(new[] { BlogMetaTestData.BlogKey }, nonExistingArchiveDate).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();

            Assert.False(anyArchiveItems);
        }

        public virtual void ListPostsByArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var archiveDate = BlogPostTestData.TestPublishedAt;

            // Act
            var archive = repository.ListPostsByArchive(new[] { "NON_EXISTING_BLOG_KEY" }, archiveDate).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();

            Assert.False(anyArchiveItems);
        }

        public virtual void ListPostsByTag_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var postQuery = repository.ListPostsByTag(new[] { BlogMetaTestData.BlogKey }, tag);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTag = posts.All(x => x.TagTexts.Contains(tag, StringComparer.OrdinalIgnoreCase));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTag);
        }

        public virtual void ListPostsByTag_ExistingBlogKeyWithWrongCasing_ReturnsPosts(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1).ToUpperInvariant();

            // Act
            var postQuery = repository.ListPostsByTag(new[] { BlogMetaTestData.BlogKey }, tag);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTag = posts.All(x => x.TagTexts.Contains(tag, StringComparer.OrdinalIgnoreCase));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTag);
        }

        public virtual void ListPostsByTag_ExistingBlogKeyAndNonExistingTag_ReturnsEmpty(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var archive = repository.ListPostsByTag(new[] { BlogMetaTestData.BlogKey }, "NON_EXISTING_TAG").ToList();

            // Assert
            bool anyArchiveItems = archive.Any();

            Assert.False(anyArchiveItems);
        }

        public virtual void ListPostsByTag_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var posts = repository.ListPostsByTag(new[] { "NON_EXISTING_BLOG_KEY" }, tag).ToList();

            // Assert
            bool anyArchiveItems = posts.Any();

            Assert.False(anyArchiveItems);
        }

        public virtual void SearchPosts_ExistingBlogKeyAndContent_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string postContent = BlogPostTestData.Create(blogKey: BlogMetaTestData.BlogKey).Content;
            postContent = postContent.Substring(
                0,
                postContent.IndexOf(Environment.NewLine, StringComparison.OrdinalIgnoreCase));

            // Act
            var posts = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, postContent).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTitle = posts.All(x => x.Content.Contains(postContent));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTitle);
        }

        public virtual void SearchPosts_ExistingBlogKeyAndTag_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var postQuery = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, tag);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTag = posts.All(x => x.TagTexts.Contains(tag));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTag);
        }

        public virtual void SearchPosts_ExistingBlogKeyAndTitle_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string postTitle = BlogPostTestData.Create().Title;

            // Act
            var posts = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, postTitle).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTitle = posts.All(x => x.Title == postTitle);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTitle);
        }

        public virtual void SearchPosts_ExistingBlogKeyAndUpperCaseTitle_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            string postTitle = BlogPostTestData.Create().Title;
            string postTitleUpper = postTitle.ToUpperInvariant();

            // Act
            var posts = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, postTitleUpper).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTitle = posts.All(x => x.Title == postTitle);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTitle);
        }

        public virtual void SearchPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = this.GetRepository(blogPosts: dbBlogPosts);

            // Act
            var archive = repository.SearchPosts(new[] { "NON_EXISTING_BLOG_KEY" }, string.Empty).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();

            Assert.False(anyArchiveItems);
        }

        public static IEnumerable<BlogMeta> GetBlogMetas()
        {
            yield return BlogMetaTestData.Create(BlogMetaTestData.BlogKey1);
            yield return BlogMetaTestData.Create(BlogMetaTestData.BlogKey2);
            yield return BlogMetaTestData.Create(BlogMetaTestData.BlogKey3);
        }

        protected abstract IRepository GetRepository(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null);

        protected static bool HasBlogPostAllFieldValues(BlogPost post)
        {
            if (post == null)
            {
                return false;
            }

            bool hasBlogPostHeadAllFieldValues = HasBlogPostHeadAllFieldValues(post);

            bool hasFieldValues = hasBlogPostHeadAllFieldValues && (post.Content != null);
            return hasFieldValues;
        }

        protected static bool HasBlogPostHeadAllFieldValues(BlogPostHead postHead)
        {
            if (postHead == null)
            {
                return false;
            }

            bool hasBlogPostBaseAllFieldValues = HasBlogPostBaseAllFieldValues(postHead);

            bool hasFieldValues = hasBlogPostBaseAllFieldValues && (postHead.BlogAuthor != null)
                                  && (postHead.BlogAuthor.ImageUrl != null) && (postHead.BlogAuthor.Name != null)
                                  && (postHead.BlogAuthor.SourceId != null) && (postHead.BlogAuthor.Url != null)
                                  && (postHead.ImageUrl != null) && (postHead.PublishedAt != null)
                                  && (postHead.SourceUrl != null) && (postHead.Summary != null)
                                  && (postHead.BlogPostTags != null) && postHead.BlogPostTags.Any()
                                  && (postHead.Title != null) && (postHead.UrlSlug != null)
                                  && (postHead.UpdatedAt != null);
            return hasFieldValues;
        }

        protected static bool HasBlogPostBaseAllFieldValues(BlogPostBase postBase)
        {
            if (postBase == null)
            {
                return false;
            }

            bool hasFieldValues = (postBase.BlavenId != null) && (postBase.BlogKey != null) && (postBase.Hash != null)
                                  && (postBase.SourceId != null);
            return hasFieldValues;
        }
    }
}
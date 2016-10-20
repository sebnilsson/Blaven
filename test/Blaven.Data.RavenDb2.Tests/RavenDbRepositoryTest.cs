using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Tests;
using Xunit;

namespace Blaven.Data.RavenDb2.Tests
{
    public class RavenDbRepositoryTest
    {
        [Fact]
        public void GetAllBlogMetas_ReturnsBlogMetasWithAllFieldValues()
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = GetRavenDbRepository(blogMetas: dbBlogMetas);

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

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey1)]
        [InlineData(BlogMetaTestData.BlogKey2)]
        [InlineData(BlogMetaTestData.BlogKey3)]
        public async Task GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta(string blogKey)
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = GetRavenDbRepository(blogMetas: dbBlogMetas);

            // Act
            var blogMeta = await repository.GetBlogMeta(blogKey);

            // Assert
            Assert.Equal(blogKey, blogMeta.BlogKey);
        }

        [Fact]
        public async Task GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var dbBlogMetas = GetBlogMetas();
            var repository = GetRavenDbRepository(blogMetas: dbBlogMetas);

            // Act
            var blogMeta = await repository.GetBlogMeta("NON_EXISTING_BLOG_KEY");

            // Assert
            Assert.Null(blogMeta);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPost_ExistingBlavenId_ReturnsPost(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost(BlogMetaTestData.BlogKey, BlogPostTestData.BlavenId2);

            // Assert
            bool hasBlogPostFieldValues = HasBlogPostAllFieldValues(post);

            Assert.Equal(BlogMetaTestData.BlogKey, post.BlogKey);
            Assert.Equal(BlogPostTestData.BlavenId2, post.BlavenId);
            Assert.True(hasBlogPostFieldValues);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPost_ExistingBlavenIdAndNonExistingBlogKey_ReturnsNull(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost("NON_EXISTING_BLOG_KEY", BlogPostTestData.BlavenId2);

            // Assert
            Assert.Null(post);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPost_NonExistingBlavenId_ReturnsNull(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPost(BlogMetaTestData.BlogKey, "NON_EXISTING_BLAVEN_ID");

            // Assert
            Assert.Null(post);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPostBySourceId_ExistingBlogKeyAndNotExistingSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, "NON_EXISTING_SOURCE_ID");

            // Assert
            Assert.Null(post);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPostBySourceId_ExistingBlogKeyAndSourceId_ReturnsBlogPost(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            string sourceId = BlogPostTestData.CreatePostSourceId(0);

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, sourceId);

            // Assert
            Assert.Equal(sourceId, post.SourceId);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPostBySourceId_ExistingBlogKeyAndUpperCaseSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            string upperCaseSourceId = BlogPostTestData.CreatePostSourceId(0).ToUpperInvariant();

            // Act
            var post = await repository.GetPostBySourceId(BlogMetaTestData.BlogKey, upperCaseSourceId);

            // Assert
            Assert.Null(post);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task GetPostBySourceId_ExistingSourceIdAndNonExistingBlogKey_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            string sourceId = BlogPostTestData.CreatePostSourceId(0);

            // Act
            var post = await repository.GetPostBySourceId("NON_EXISTING_BLOG_KEY", sourceId);

            // Assert
            Assert.Null(post);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListArchive_ExistingBlogKey_ReturnsArchive(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

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

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var archive = repository.ListArchive(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListTags_ExistingBlogKey_ReturnsListTags(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

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

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListTags_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var tags = repository.ListTags(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyTagItems = tags.Any();
            Assert.False(anyTagItems);
        }

        [Fact]
        public void ListTags_DifferentCasingTags_ReturnsGroupedTags()
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
                                Tags = new[] { x }
                            }).ToList();

            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var tags = repository.ListTags(new[] { BlogMetaTestData.BlogKey }).ToList();

            // Assert
            var testTagItem = tags.FirstOrDefault(x => x.Name == "Test tag");
            Assert.NotNull(testTagItem);
            Assert.Equal(5, testTagItem.Count);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostHeads_ExistingBlogKey_ReturnsPostHeads(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var postHeadsQuery = repository.ListPostHeads(new[] { BlogMetaTestData.BlogKey });

            var postHeads = postHeadsQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostHeadsMatchBlogKey = postHeads.Any() && postHeads.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = postHeads.All(HasBlogPostHeadAllFieldValues);

            Assert.True(allPostHeadsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostHeads_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var postHeads = repository.ListPostHeads(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyPostHeads = postHeads.Any();
            Assert.False(anyPostHeads);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPosts_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var postQuery = repository.ListPosts(new[] { BlogMetaTestData.BlogKey });

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var posts = repository.ListPosts(new[] { "NON_EXISTING_BLOG_KEY" }).ToList();

            // Assert
            bool anyArchiveItems = posts.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByArchive_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

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

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByArchive_ExistingBlogKeyAndNonExistingArchiveDate_ReturnsEmpty(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            var nonExistingArchiveDate = BlogPostTestData.TestPublishedAt.AddYears(-1);

            // Act
            var archive =
                repository.ListPostsByArchive(new[] { BlogMetaTestData.BlogKey }, nonExistingArchiveDate).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            var archiveDate = BlogPostTestData.TestPublishedAt;

            // Act
            var archive = repository.ListPostsByArchive(new[] { "NON_EXISTING_BLOG_KEY" }, archiveDate).ToList();

            // Assert
            bool anyArchiveItems = archive.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByTag_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var postQuery = repository.ListPostsByTag(new[] { BlogMetaTestData.BlogKey }, tag);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTag = posts.All(x => x.Tags.Contains(tag));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTag);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByTag_ExistingBlogKeyAndNonExistingTag_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            // Act
            var archive = repository.ListPostsByTag(new[] { BlogMetaTestData.BlogKey }, "NON_EXISTING_TAG").ToList();

            // Assert
            bool anyArchiveItems = archive.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void ListPostsByTag_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var posts = repository.ListPostsByTag(new[] { "NON_EXISTING_BLOG_KEY" }, tag).ToList();

            // Assert
            bool anyArchiveItems = posts.Any();
            Assert.False(anyArchiveItems);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void SearchPosts_ExistingBlogKeyAndContent_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            string postContent = BlogPostTestData.Create(BlogMetaTestData.BlogKey).Content;

            // Act
            var posts = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, postContent).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTitle = posts.All(x => x.Content == postContent);

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTitle);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void SearchPosts_ExistingBlogKeyAndTag_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            var tag = BlogPostTestData.CreatePostTag(1);

            // Act
            var postQuery = repository.SearchPosts(new[] { BlogMetaTestData.BlogKey }, tag);

            var posts = postQuery.Skip(0).Take(100).ToList();

            // Assert
            bool allPostsMatchBlogKey = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey);
            bool allHasBlogPostFieldValues = posts.All(HasBlogPostAllFieldValues);
            bool allPostsContainsTag = posts.All(x => x.Tags.Contains(tag));

            Assert.True(allPostsMatchBlogKey);
            Assert.True(allHasBlogPostFieldValues);
            Assert.True(allPostsContainsTag);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void SearchPosts_ExistingBlogKeyAndTitle_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

            string postTitle = BlogPostTestData.Create(BlogMetaTestData.BlogKey).Title;

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

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public void SearchPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var repository = GetRavenDbRepository(blogPosts: dbBlogPosts);

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

        private static RavenDbRepository GetRavenDbRepository(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null)
        {
            var documentStore = EmbeddableDocumentStoreHelper.GetWithData(blogMetas: blogMetas, blogPosts: blogPosts);

            var repository = new RavenDbRepository(documentStore);
            return repository;
        }

        private static bool HasBlogPostAllFieldValues(BlogPost post)
        {
            if (post == null)
            {
                return false;
            }

            bool hasBlogPostHeadAllFieldValues = HasBlogPostHeadAllFieldValues(post);

            bool hasFieldValues = hasBlogPostHeadAllFieldValues && (post.Content != null);
            return hasFieldValues;
        }

        private static bool HasBlogPostHeadAllFieldValues(BlogPostHead postHead)
        {
            if (postHead == null)
            {
                return false;
            }

            bool hasBlogPostBaseAllFieldValues = HasBlogPostBaseAllFieldValues(postHead);

            bool hasFieldValues = hasBlogPostBaseAllFieldValues && (postHead.Author != null)
                                  && (postHead.Author.ImageUrl != null) && (postHead.Author.Name != null)
                                  && (postHead.Author.SourceId != null) && (postHead.Author.Url != null)
                                  && (postHead.ImageUrl != null) && (postHead.PublishedAt != null)
                                  && (postHead.SourceUrl != null) && (postHead.Summary != null)
                                  && (postHead.Tags != null) && postHead.Tags.Any() && (postHead.Title != null)
                                  && (postHead.UrlSlug != null) && (postHead.UpdatedAt != null);
            return hasFieldValues;
        }

        private static bool HasBlogPostBaseAllFieldValues(BlogPostBase postBase)
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
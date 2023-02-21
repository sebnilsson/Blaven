using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.InMemory;
using Blaven.Testing;
using Blaven.Transformation;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class BlogSyncServiceTest
    {
        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
            var storagePosts = BlogPostTestFactory.CreateList(2, 3);

            var syncService = GetBlogSyncService(blogSourcePosts, storagePosts);

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Equal(2, result.Posts.Inserted.Count);
            Assert.Empty(result.Posts.Updated);
            Assert.Empty(result.Posts.Deleted);
        }

        [Theory]
        [MemberData(nameof(GetChangedBlogPostData))]
        public async Task Synchronize_ContainsHashUpdates_ReturnsUpdates(
            BlogPost[] blogSourcePosts,
            int expectedUpdateCount)
        {
            // Arrange
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetBlogSyncService(blogSourcePosts, storagePosts);

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Equal(expectedUpdateCount, result.Posts.Updated.Count);
            Assert.Empty(result.Posts.Inserted);
            Assert.Empty(result.Posts.Deleted);
        }

        [Fact]
        public async Task Synchronize_ContainsDeleted_ReturnsDeleted()
        {
            // Arrange
            var blogSourcePosts = BlogPostTestFactory.CreateList(2, 3);
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetBlogSyncService(blogSourcePosts, storagePosts);

            // Act
            var result = await syncService.Synchronize();

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Empty(result.BlogPostsChanges.DeletedBlogPosts);
            Assert.Equal(2, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(2, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Fact]
        public async Task Synchronize_NoChanges_ReturnsNoChanges()
        {
            // Arrange
            var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetBlogSyncService(blogSourcePosts, storagePosts);

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Empty(result.Posts.Inserted);
            Assert.Empty(result.Posts.Updated);
            Assert.Empty(result.Posts.Deleted);
        }

        public static IEnumerable<object[]> GetChangedBlogPostData()
        {
            // Arrange
            var service = BlogSyncServiceTestFactory.Create();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.UpdateAll("NON_EXISTING_BLOGKEY"));
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, true)]
        [InlineData(BlogMetaTestData.BlogKey2, false)]
        public async Task UpdateAll_BlogSourceBlogMetaChange_ReturnsBlogMeta(string blogKey, bool expectNotNull)
        {
            // Arrange
            var blogSourceMetas = new[]
                                  {
                                      BlogMetaTestData.Create(blogKey),
                                      BlogMetaTestData.Create(BlogMetaTestData.BlogKey3)
                                  };

            var service = BlogSyncServiceTestFactory.CreateWithData(blogSourceMetas: blogSourceMetas);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            var isExpectedNull = expectNotNull ? result.BlogMeta != null : result.BlogMeta == null;

            Assert.True(isExpectedNull);
        }

        [Fact]
        public async Task UpdateAll_BlogSourceChangesInOtherBlogKey_ReturnsNoChanges()
        {
            // Arrange
            var dataStoragePosts = new[] { BlogPostTestData.Create(1), BlogPostTestData.Create(3) };
            var blogSourcePosts = new[]
                                  {
                                      BlogPostTestData.Create(0), BlogPostTestData.Create(1),
                                      BlogPostTestData.Create(2)
                                  };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey2);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey2);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceDeletedPosts_ReturnsDeletedPosts(
            string blogKey,
            int expectedDeletedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 4, blogKey);
            var blogSourcePosts = BlogPostTestData.CreateCollection(0, 2, blogKey);

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(expectedDeletedBlogPosts, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Fact]
        public async Task UpdateAll_BlogSourceInsertedAndUpdatedPostsWithMultipleBlogKeys_ReturnsAllChanges()
        {
            // Arrange
            var dataStoragePosts1 = BlogPostTestData.CreateCollection(0, 4, BlogMetaTestData.BlogKey1);
            var dataStoragePosts2 = BlogPostTestData.CreateCollection(3, 4, BlogMetaTestData.BlogKey2);
            var dataStoragePosts3 = BlogPostTestData.CreateCollection(6, 4, BlogMetaTestData.BlogKey3);

            var blogSourcePosts1 = BlogPostTestData.CreateCollection(2, 4, BlogMetaTestData.BlogKey1, "CHANGED_");
            var blogSourcePosts2 = BlogPostTestData.CreateCollection(5, 4, BlogMetaTestData.BlogKey2, "CHANGED_");
            var blogSourcePosts3 = BlogPostTestData.CreateCollection(8, 4, BlogMetaTestData.BlogKey3, "CHANGED_");

            var service = BlogSyncServiceTestFactory.CreateWithData(
                dataStoragePosts1.Concat(dataStoragePosts2).Concat(dataStoragePosts3),
                dataStoragePosts: blogSourcePosts1.Concat(blogSourcePosts2).Concat(blogSourcePosts3));

            // Act
            var results = await service.UpdateAll();

            // Assert
            var testDataBlogKeys =
                BlogSettingTestData.CreateCollection().Select(x => x.BlogKey).OrderBy(x => x).ToList();
            var resultBlogKeys = results.Select(x => x.BlogKey).OrderBy(x => x).ToList();

            var result1 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey1);
            var result2 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey2);
            var result3 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey3);

            var resultBlogKeysReferenceEquals = resultBlogKeys.SequenceEqual(testDataBlogKeys);

            Assert.True(resultBlogKeysReferenceEquals);

            Assert.Equal(2, result1.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(2, result1.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(2, result1.BlogPostsChanges.UpdatedBlogPosts.Count);

            Assert.Equal(2, result2.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(2, result2.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(2, result2.BlogPostsChanges.UpdatedBlogPosts.Count);

            Assert.Equal(2, result3.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(2, result3.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(2, result3.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceInsertedPosts_ReturnsInsertedPosts(
            string blogKey,
            int expectedInsertedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 2, blogKey);
            var blogSourcePosts = BlogPostTestData.CreateCollection(0, 4, blogKey);

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(expectedInsertedBlogPosts, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceUpdatedPostHashs_ReturnsUpdatedPosts(
            string blogKey,
            int expectedUpdatedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 2, blogKey);
            var blogSourcePosts = new[]
                                  {
                                      BlogPostTestData.Create(0, blogKey, hashPrefix: "CHANGED_"),
                                      BlogPostTestData.Create(1, blogKey, hashPrefix: "CHANGED_")
                                  };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(expectedUpdatedBlogPosts, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        private static BlogPost[] CreateChangedBlogPosts(
            Action<BlogPost> config)
        {
            return
                new BlogPost[]
                {
                    BlogPostTestFactory.Create(1),
                    BlogPostTestFactory.Create(2, config),
                    BlogPostTestFactory.Create(3, config),
                    BlogPostTestFactory.Create(4)
                };
        }

        private IBlogSyncService GetBlogSyncService(
            IReadOnlyList<BlogPost>? blogSourcePosts = null,
            IReadOnlyList<BlogPost>? storagePosts = null)
        {
            var blogSource = new FakeBlogSource(blogSourcePosts);

            var inMemoryStorage = new InMemoryStorage(
                Enumerable.Empty<BlogMeta>(),
                storagePosts ?? Enumerable.Empty<BlogPost>());

            var storageSyncRepo =
                new InMemoryStorageSyncRepository(inMemoryStorage);

            var transformService =
                new BlogPostStorageTransformService(
                    Enumerable.Empty<IBlogPostStorageTransform>());

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(expectedUpdatedBlogPosts, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }
    }
}

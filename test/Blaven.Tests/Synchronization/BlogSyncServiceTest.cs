using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;
using Blaven.Tests;
using Moq;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class BlogSyncServiceTest
    {
        [Fact]
        public async Task Update_BlogSourceGetMetaAndGetBlogPostsReturnsNull_ShouldNotThrow()
        {
            // Arrange
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x => x.GetMeta(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>())).ReturnsAsync(null);
            blogSource.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>())).ReturnsAsync(null);

            var service = BlogSyncServiceTestFactory.Create(blogSource: blogSource.Object);

            // Act & Assert
            await service.Update();
        }

        [Fact]
        public async Task Update_BlogSourceGetMetaThrows_ShouldThrow()
        {
            // Arrange
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x => x.GetMeta(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .Throws(new Exception("TEST_ERROR"));

            var service = BlogSyncServiceTestFactory.Create(blogSource: blogSource.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncBlogSourceException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_BlogSourceGetBlogPostsThrows_ShouldThrow()
        {
            // Arrange
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .Throws(new Exception("TEST_ERROR"));

            var service = BlogSyncServiceTestFactory.Create(blogSource: blogSource.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncBlogSourceException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_DataStorageGetBlogPostsReturnsNull_ShouldThrow()
        {
            // Arrange
            var dataStorage = new Mock<IDataStorage>();
            dataStorage.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>())).ReturnsAsync(null);

            var service = BlogSyncServiceTestFactory.Create(dataStorage: dataStorage.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncDataStorageResultException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_DataStorageGetBlogPostsThrows_ShouldThrow()
        {
            // Arrange
            var dataStorage = new Mock<IDataStorage>();
            dataStorage.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .Throws(new Exception("TEST_ERROR"));

            var service = BlogSyncServiceTestFactory.Create(dataStorage: dataStorage.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncDataStorageException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_BlogKeyNotExisting_ShouldThrow()
        {
            // Arrange
            var service = BlogSyncServiceTestFactory.Create();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.Update("NON_EXISTING_BLOGKEY"));
        }

        [Fact]
        public async Task UpdateAll_BlogKeyNotExisting_ShouldThrow()
        {
            // Arrange
            var service = BlogSyncServiceTestFactory.Create();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.UpdateAll("NON_EXISTING_BLOGKEY"));
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceDeletedPosts_ReturnsDeletedPosts(
            string blogKey,
            int expectedDeletedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(start: 0, count: 4, blogKey: blogKey);
            var blogSourcePosts = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: blogKey);

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(expectedDeletedBlogPosts, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceInsertedPosts_ReturnsInsertedPosts(
            string blogKey,
            int expectedInsertedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: blogKey);
            var blogSourcePosts = BlogPostTestData.CreateCollection(start: 0, count: 4, blogKey: blogKey);

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
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
            var dataStoragePosts = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: blogKey);
            var blogSourcePosts = new[]
                                      {
                                          BlogPostTestData.Create(blogKey, index: 0, hashPrefix: "CHANGED_"),
                                          BlogPostTestData.Create(blogKey, index: 1, hashPrefix: "CHANGED_")
                                      };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(expectedUpdatedBlogPosts, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Fact]
        public async Task UpdateAll_BlogSourceUpdatedPostTags_ReturnsUpdatedPostWithUpdatedTags()
        {
            // Arrange
            const int UpdatedTagCount = BlogPostTestData.DefaultTagCount + 2;

            var dataStoragePost = BlogPostTestData.Create(index: 0);

            var dataStoragePosts = new[] { dataStoragePost };
            var blogSourcePosts = new[]
                                      {
                                          BlogPostTestData.Create(
                                              index: 0,
                                              hashPrefix: "_CHANGED",
                                              tagCount: UpdatedTagCount)
                                      };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);
            var updatedPost = result.BlogPostsChanges.UpdatedBlogPosts.First();

            Assert.Equal(BlogPostTestData.DefaultTagCount, dataStoragePost.Tags.Count());
            Assert.Equal(UpdatedTagCount, updatedPost.Tags.Count());
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceUpdatedPostUpdatedAts_ReturnsUpdatedPosts(
            string blogKey,
            int expectedUpdatedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: blogKey);
            var blogSourcePosts = new[]
                                      {
                                          BlogPostTestData.Create(blogKey, index: 0, updatedAtAddedDays: 1),
                                          BlogPostTestData.Create(blogKey, index: 1, updatedAtAddedDays: 2)
                                      };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(0, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(expectedUpdatedBlogPosts, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Fact]
        public async Task UpdateAll_BlogSourceChangesInOtherBlogKey_ReturnsNoChanges()
        {
            // Arrange
            var dataStoragePosts = new[] { BlogPostTestData.Create(index: 1), BlogPostTestData.Create(index: 3) };
            var blogSourcePosts = new[]
                                      {
                                          BlogPostTestData.Create(index: 0), BlogPostTestData.Create(index: 1),
                                          BlogPostTestData.Create(index: 2)
                                      };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts: blogSourcePosts,
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

            bool isExpectedNull = expectNotNull ? (result.BlogMeta != null) : (result.BlogMeta == null);

            Assert.True(isExpectedNull);
        }

        [Fact]
        public async Task UpdateAll_BlogSourceInsertedAndUpdatedPostsWithMultipleBlogKeys_ReturnsAllChanges()
        {
            // Arrange
            var dataStoragePosts1 = BlogPostTestData.CreateCollection(
                start: 0,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey1);
            var dataStoragePosts2 = BlogPostTestData.CreateCollection(
                start: 3,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey2);
            var dataStoragePosts3 = BlogPostTestData.CreateCollection(
                start: 6,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey3);

            var blogSourcePosts1 = BlogPostTestData.CreateCollection(
                start: 2,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey1,
                hashPrefix: "CHANGED_");
            var blogSourcePosts2 = BlogPostTestData.CreateCollection(
                start: 5,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey2,
                hashPrefix: "CHANGED_");
            var blogSourcePosts3 = BlogPostTestData.CreateCollection(
                start: 8,
                count: 4,
                blogKey: BlogMetaTestData.BlogKey3,
                hashPrefix: "CHANGED_");

            var service =
                BlogSyncServiceTestFactory.CreateWithData(
                    blogSourcePosts: dataStoragePosts1.Concat(dataStoragePosts2).Concat(dataStoragePosts3),
                    dataStoragePosts: blogSourcePosts1.Concat(blogSourcePosts2).Concat(blogSourcePosts3));

            // Act
            var results = await service.UpdateAll();

            // Assert
            var testDataBlogKeys = BlogSettingTestData.CreateCollection().Select(x => x.BlogKey).OrderBy(x => x).ToList();
            var resultBlogKeys = results.Select(x => x.BlogKey).OrderBy(x => x).ToList();

            var result1 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey1);
            var result2 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey2);
            var result3 = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey3);

            bool resultBlogKeysReferenceEquals = resultBlogKeys.SequenceEqual(testDataBlogKeys);

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
    }
}
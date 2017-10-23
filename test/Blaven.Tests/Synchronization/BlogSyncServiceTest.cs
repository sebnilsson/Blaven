using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSources;
using Blaven.DataStorage;
using Blaven.Testing;
using Moq;
using Xunit;

namespace Blaven.Synchronization.Testing
{
    public class BlogSyncServiceTest
    {
        [Fact]
        public async Task Update_BlogKeyNotExisting_ShouldThrow()
        {
            // Arrange
            var service = BlogSyncServiceTestFactory.Create();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.Update("NON_EXISTING_BLOGKEY"));
        }

        [Fact]
        public async Task Update_BlogSourceGetBlogPostsThrows_ShouldThrow()
        {
            // Arrange
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .Throws(new Exception("TEST_ERROR"));

            var service = BlogSyncServiceTestFactory.Create(blogSource.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncBlogSourceException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_BlogSourceGetMetaAndGetBlogPostsReturnsNull_ShouldNotThrow()
        {
            // Arrange
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x => x.GetMeta(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((BlogMeta)null);
            blogSource.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((IReadOnlyList<BlogPost>)null);

            var service = BlogSyncServiceTestFactory.Create(blogSource.Object);

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

            var service = BlogSyncServiceTestFactory.Create(blogSource.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BlogSyncBlogSourceException>(async () => await service.Update());
        }

        [Fact]
        public async Task Update_DataStorageContainsPosts_ShouldUpdateNewAndNotFlagOldAsDeleted()
        {
            // Arrange
            var dataStoragePosts = new[]
                                   {
                                       BlogPostTestData.Create(0, updatedAt: new DateTime(2016, 1, 1)),
                                       BlogPostTestData.Create(1, updatedAt: new DateTime(2016, 2, 2)),
                                       BlogPostTestData.Create(2, updatedAt: new DateTime(2016, 3, 3)),
                                       BlogPostTestData.Create(3, updatedAt: new DateTime(2016, 4, 4))
                                   };
            var blogSourcePosts = new[]
                                  {
                                      BlogPostTestData.Create(
                                          0,
                                          updatedAt: new DateTime(2016, 1, 1),
                                          hashPrefix: "CHANGED_"),
                                      BlogPostTestData.Create(
                                          1,
                                          updatedAt: new DateTime(2016, 2, 2),
                                          hashPrefix: "CHANGED_"),
                                      BlogPostTestData.Create(2, updatedAt: new DateTime(2016, 3, 3)),
                                      BlogPostTestData.Create(3, updatedAt: new DateTime(2016, 4, 4)),
                                      BlogPostTestData.Create(4, updatedAt: new DateTime(2016, 5, 5)),
                                      BlogPostTestData.Create(5, updatedAt: new DateTime(2016, 6, 6))
                                  };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(0, result.BlogPostsChanges.DeletedBlogPosts.Count);
            Assert.Equal(2, result.BlogPostsChanges.InsertedBlogPosts.Count);
            Assert.Equal(2, result.BlogPostsChanges.UpdatedBlogPosts.Count);
        }

        [Fact]
        public async Task Update_DataStorageGetBlogPostsReturnsNull_ShouldThrow()
        {
            // Arrange
            var dataStorage = new Mock<IDataStorage>();
            dataStorage.Setup(x => x.GetBlogPosts(It.IsAny<BlogSetting>(), It.IsAny<DateTime?>()))
                .ReturnsAsync((IReadOnlyList<BlogPostBase>)null);

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
        public async Task UpdateAll_BlogKeyNotExisting_ShouldThrow()
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

        [Fact]
        public async Task UpdateAll_BlogSourceUpdatedPostTags_ReturnsUpdatedPostWithUpdatedTags()
        {
            // Arrange
            const int updatedTagCount = BlogPostTestData.DefaultTagCount + 2;

            var dataStoragePost = BlogPostTestData.Create(0);

            var dataStoragePosts = new[] { dataStoragePost };
            var blogSourcePosts =
                new[] { BlogPostTestData.Create(0, hashPrefix: "_CHANGED", tagCount: updatedTagCount) };

            var service = BlogSyncServiceTestFactory.CreateWithData(
                blogSourcePosts,
                dataStoragePosts: dataStoragePosts);

            // Act
            var results = await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            var result = results.First(x => x.BlogKey == BlogMetaTestData.BlogKey);
            var updatedPost = result.BlogPostsChanges.UpdatedBlogPosts.First();

            Assert.Equal(BlogPostTestData.DefaultTagCount, dataStoragePost.BlogPostTags.Count);
            Assert.Equal(updatedTagCount, updatedPost.BlogPostTags.Count);
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey, 2)]
        [InlineData(BlogMetaTestData.BlogKey2, 0)]
        public async Task UpdateAll_BlogSourceUpdatedPostUpdatedAts_ReturnsUpdatedPosts(
            string blogKey,
            int expectedUpdatedBlogPosts)
        {
            // Arrange
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 2, blogKey);
            var blogSourcePosts = new[]
                                  {
                                      BlogPostTestData.Create(0, blogKey, updatedAtAddedDays: 1),
                                      BlogPostTestData.Create(1, blogKey, updatedAtAddedDays: 2)
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
    }
}
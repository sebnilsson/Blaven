using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;
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

        //[Fact]
        //public async Task GetBlogPosts_BlogSourceInsertedPosts_ReturnsInsertedPosts()
        //{
        //    // Arrange
        //    var dataStoragePosts = GetTestBlogPosts(0, 1).ToList();

        //    var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts(0, 3));

        //    var blogSetting = GetTestBlogSetting();

        //    // Act
        //    var blogPosts = await bloggerBlogSource.GetBlogPosts(blogSetting, dataStoragePosts, lastUpdatedAt: null);

        //    // Assert
        //    Assert.Equal(2, blogPosts.Count);
        //}

        //[Fact]
        //public async Task GetBlogPosts_BlogSourceDeletedPosts_ReturnsDeletedPosts()
        //{
        //    // Arrange
        //    var dataStoragePosts = GetTestBlogPosts(0, 3).ToList();

        //    var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts(0, 1));

        //    var blogSetting = GetTestBlogSetting();

        //    // Act
        //    var blogPosts = await bloggerBlogSource.GetBlogPosts(blogSetting, dataStoragePosts, lastUpdatedAt: null);

        //    // Assert
        //    Assert.Equal(2, blogPosts.Count);
        //}

        //[Fact]
        //public async Task GetBlogPosts_BlogSourceUpdatedPosts_ReturnsUpdatedPosts()
        //{
        //    // Arrange
        //    var dataStoragePosts = GetTestBlogPosts(0, 2).ToList();

        //    var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => GetTestModifiedPosts(0, 2));

        //    var blogSetting = GetTestBlogSetting();

        //    // Act
        //    var blogPosts = await bloggerBlogSource.GetBlogPosts(blogSetting, dataStoragePosts, lastUpdatedAt: null);

        //    // Assert
        //    Assert.Equal(2, blogPosts.Count);
        //}

        //[Fact]
        //public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoDeletedPosts()
        //{
        //    // Arrange
        //    var sourcePosts = BlogPostTestData.CreateCollection(0, 0).ToList();
        //    var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dbPostList = dataStoragePosts.OfType<BlogPostBase>().ToList();
        //    var lastUpdatedAt = dataStoragePosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

        //    // Act
        //    var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dbPostList);

        //    // Assert
        //    Assert.Equal(0, changeSet.DeletedBlogPosts.Count);
        //}

        //[Fact]
        //public void GetChangeSet_NoLastUpdatedAt_AllPostsDeleted()
        //{
        //    // Arrange
        //    var sourcePosts = BlogPostTestData.CreateCollection(0, 0).ToList();
        //    var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();

        //    // Act
        //    var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

        //    // Assert
        //    Assert.Equal(5, changeSet.DeletedBlogPosts.Count);
        //}

        //[Fact]
        //public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoUpdatedBlogPosts()
        //{
        //    // Arrange
        //    var sourcePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();
        //    var lastUpdatedAt = dataStoragePosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

        //    // Act
        //    var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

        //    // Assert
        //    Assert.Equal(0, changeSet.UpdatedBlogPosts.Count);
        //}

        //[Fact]
        //public void GetChangeSet_NoLastUpdatedAt_AllPostsUpdated()
        //{
        //    // Arrange
        //    var sourcePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
        //    var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();

        //    // Act
        //    var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

        //    // Assert
        //    Assert.Equal(5, changeSet.UpdatedBlogPosts.Count);
        //}
    }
}
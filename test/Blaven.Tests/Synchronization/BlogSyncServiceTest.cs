using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Blaven.BlogSources.Tests;
using Blaven.Data.Tests;
using Blaven.Tests;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class SynchronizationServiceTest
    {
        //[Fact]
        //public void Update()
        //{
        //    // Arrange
            

        //    // Act

        //    // Assert
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

        [Fact]
        public void Update_ParallelUsersAndSingleBlogKey_ShouldRunAllMethodsOncePerUserAndWithoutCollisions()
        {
            // Arrange
            var blogSource = new FakeBlogSource(
                                 blogPosts: BlogPostTestData.CreateCollection(),
                                 blogMetas: BlogMetaTestData.CreateCollection());

            var dataStorage = new FakeDataStorage(blogPosts: BlogPostTestData.CreateCollection());

            var service = BlogSyncServiceTestFactory.Create(blogSource, dataStorage);

            var blogSourceGetMetaObserver = GetTestEventObserver();
            var blogSourceGetBlogPostsObserver = GetTestEventObserver();
            blogSource.OnGetaMetaRun += blogSourceGetMetaObserver.Handler;
            blogSource.OnGetBlogPostsRun += blogSourceGetBlogPostsObserver.Handler;

            var dataStorageGetLastUpdatedAtObserver = GetTestEventObserver();
            var dataStorageGetPostBasesObserver = GetTestEventObserver();
            var dataStorageSaveBlogMetaObserver = GetTestEventObserver();
            var dataStorageSaveChangesObserver = GetTestEventObserver();
            dataStorage.OnGetLastUpdatedAtRun += dataStorageGetLastUpdatedAtObserver.Handler;
            dataStorage.OnGetPostBasesRun += dataStorageGetPostBasesObserver.Handler;
            dataStorage.OnSaveBlogMetaRun += dataStorageSaveBlogMetaObserver.Handler;
            dataStorage.OnSaveChangesRun += dataStorageSaveChangesObserver.Handler;

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.Update(BlogMetaTestData.BlogKey));

            // Assert
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, blogSourceGetMetaObserver.RunCount);
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, blogSourceGetBlogPostsObserver.RunCount);
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, dataStorageGetLastUpdatedAtObserver.RunCount);
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, dataStorageGetPostBasesObserver.RunCount);
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, dataStorageSaveBlogMetaObserver.RunCount);
            Assert.Equal(ParallelUtility.DefaultParallelUsersCount, dataStorageSaveChangesObserver.RunCount);

            int totalCollisionCount = blogSourceGetMetaObserver.CollisionCount
                                      + blogSourceGetBlogPostsObserver.CollisionCount
                                      + dataStorageGetLastUpdatedAtObserver.CollisionCount
                                      + dataStorageGetPostBasesObserver.CollisionCount
                                      + dataStorageSaveBlogMetaObserver.CollisionCount
                                      + dataStorageSaveChangesObserver.CollisionCount;

            Assert.Equal(0, totalCollisionCount);
        }

        [Fact]
        public void Update_ParallelUsersAndMultipleBlogKeys_ShouldRunAllMethodsOncePerUserAndWithoutCollisions()
        {
            // Arrange
            var blogSource = new FakeBlogSource(
                                 blogPosts: BlogPostTestData.CreateCollection(),
                                 blogMetas: BlogMetaTestData.CreateCollection());

            var dataStorage = new FakeDataStorage(blogPosts: BlogPostTestData.CreateCollection());

            var service = BlogSyncServiceTestFactory.Create(blogSource, dataStorage);

            var blogSourceGetMetaObserver = GetTestEventObserver();
            var blogSourceGetBlogPostsObserver = GetTestEventObserver();
            blogSource.OnGetaMetaRun += blogSourceGetMetaObserver.Handler;
            blogSource.OnGetBlogPostsRun += blogSourceGetBlogPostsObserver.Handler;

            var dataStorageGetLastUpdatedAtObserver = GetTestEventObserver();
            var dataStorageGetPostBasesObserver = GetTestEventObserver();
            var dataStorageSaveBlogMetaObserver = GetTestEventObserver();
            var dataStorageSaveChangesObserver = GetTestEventObserver();
            dataStorage.OnGetLastUpdatedAtRun += dataStorageGetLastUpdatedAtObserver.Handler;
            dataStorage.OnGetPostBasesRun += dataStorageGetPostBasesObserver.Handler;
            dataStorage.OnSaveBlogMetaRun += dataStorageSaveBlogMetaObserver.Handler;
            dataStorage.OnSaveChangesRun += dataStorageSaveChangesObserver.Handler;

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.Update(BlogMetaTestData.BlogKeys));

            // Assert
            int expectedRunCount = (ParallelUtility.DefaultParallelUsersCount * BlogMetaTestData.BlogKeys.Length);

            Assert.Equal(expectedRunCount, blogSourceGetMetaObserver.RunCount);
            Assert.Equal(expectedRunCount, blogSourceGetBlogPostsObserver.RunCount);
            Assert.Equal(expectedRunCount, dataStorageGetLastUpdatedAtObserver.RunCount);
            Assert.Equal(expectedRunCount, dataStorageGetPostBasesObserver.RunCount);
            Assert.Equal(expectedRunCount, dataStorageSaveBlogMetaObserver.RunCount);
            Assert.Equal(expectedRunCount, dataStorageSaveChangesObserver.RunCount);

            int totalCollisionCount = blogSourceGetMetaObserver.CollisionCount
                                      + blogSourceGetBlogPostsObserver.CollisionCount
                                      + dataStorageGetLastUpdatedAtObserver.CollisionCount
                                      + dataStorageGetPostBasesObserver.CollisionCount
                                      + dataStorageSaveBlogMetaObserver.CollisionCount
                                      + dataStorageSaveChangesObserver.CollisionCount;

            Assert.Equal(0, totalCollisionCount);
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

        [Fact]
        public async Task UpdateAll_BlogSourceAndDataStorageWithData_ShouldNotRunDataStorageGetLastUpdated()
        {
            // Arrange
            var blogSource = new FakeBlogSource(
                                 blogPosts: BlogPostTestData.CreateCollection(),
                                 blogMetas: BlogMetaTestData.CreateCollection());

            var dataStorage = new FakeDataStorage(blogPosts: BlogPostTestData.CreateCollection());

            var service = BlogSyncServiceTestFactory.Create(blogSource, dataStorage);

            var dataStorageGetLastUpdatedAtObserver = new EventObserver((_, __) => { Thread.Sleep(200); });
            dataStorage.OnGetLastUpdatedAtRun += dataStorageGetLastUpdatedAtObserver.Handler;

            // Act
            await service.UpdateAll(BlogMetaTestData.BlogKey);

            // Assert
            Assert.Equal(0, dataStorageGetLastUpdatedAtObserver.RunCount);
        }

        private static EventObserver GetTestEventObserver(int threadSleep = 100)
        {
            var observer = new EventObserver((_, __) => { Thread.Sleep(threadSleep); });
            return observer;
        }
    }
}
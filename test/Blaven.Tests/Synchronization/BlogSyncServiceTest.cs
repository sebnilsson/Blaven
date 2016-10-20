using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.BlogSources.Tests;
using Blaven.Data;
using Blaven.Data.Tests;
using Blaven.Tests;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class SynchronizationServiceTest
    {
        [Fact]
        public void Update()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act

            // Assert
        }

        [Fact]
        public void UpdateAll_ParallelUsersAndSingleBlogKey_ShouldNotRunWithCollsion()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.UpdateAll(BlogMetaTestData.BlogKey));

            // Assert
            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.False(anyTrackersWithCollision);
        }

        [Fact]
        public void UpdateAll_ParallelUsersAndSingleBlogKey_ShouldRunAllBlogSourceAndDataStorage()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.UpdateAll(BlogMetaTestData.BlogKey));

            // Assert
            bool allTrackersHasRunAll =
                AllTrackers(tracker => tracker.RunCount == ParallelUtility.DefaultParallelUsersCount, service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public void UpdateAll_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorage()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.UpdateAll(BlogMetaTestData.BlogKeys));

            // Assert
            int expectedRunCount = (ParallelUtility.DefaultParallelUsersCount * BlogMetaTestData.BlogKeys.Length);

            bool allTrackersHasRunAll = AllTrackers(tracker => tracker.RunCount == expectedRunCount, service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public async Task Update_BlogKeyNotExisting_ShouldThrow()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act & Assert
            await
                Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    async () => await service.Update("NON_EXISTING_BLOGKEY"));
        }

        [Theory]
        [MemberData(nameof(GetMultipleBlogKeys))]
        [MemberData(nameof(GetSingleBlogKey))]
        public void Update_MultipleOrSingleBlogKey_ShouldNotRunWithAnyCollsion(string[] blogKeys)
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            ParallelUtility.RunParallelUsers(async () => await service.Update(blogKeys));

            // Assert
            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.False(anyTrackersWithCollision);
        }

        public static IEnumerable<object[]> GetMultipleBlogKeys()
        {
            yield return new object[] { BlogMetaTestData.BlogKeys };
        }

        public static IEnumerable<object[]> GetSingleBlogKey()
        {
            yield return new object[] { new[] { BlogMetaTestData.BlogKey }.ToArray() };
        }

        private static bool AllTrackers(Func<DelegateTracker<BlogSetting>, bool> predicate, BlogSyncService service)
        {
            var trackers = GetServiceTrackers(service);

            bool allTrackers = AllTrackers(predicate, trackers);
            return allTrackers;
        }

        private static bool AllTrackers(
            Func<DelegateTracker<BlogSetting>, bool> predicate,
            params DelegateTracker<BlogSetting>[] trackers)
        {
            bool allTrackers = trackers.All(predicate);
            return allTrackers;
        }

        private static bool AnyTrackers(Func<DelegateTracker<BlogSetting>, bool> predicate, BlogSyncService service)
        {
            var trackers = GetServiceTrackers(service);

            bool allTrackers = AnyTrackers(predicate, trackers);
            return allTrackers;
        }

        private static bool AnyTrackers(
            Func<DelegateTracker<BlogSetting>, bool> predicate,
            params DelegateTracker<BlogSetting>[] trackers)
        {
            bool allTrackers = trackers.Any(predicate);
            return allTrackers;
        }

        private static DelegateTracker<BlogSetting>[] GetServiceTrackers(BlogSyncService service)
        {
            var trackers = GetServiceTrackersInternal(service).Where(x => x != null).ToArray();
            return trackers;
        }

        private static IEnumerable<DelegateTracker<BlogSetting>> GetServiceTrackersInternal(BlogSyncService service)
        {
            var blogSource = service.Config.BlogSource as MockBlogSource;
            var dataStorage = service.Config.DataStorage as MockDataStorage;

            if (blogSource != null)
            {
                yield return blogSource.GetChangesTracker;
                yield return blogSource.GetMetaTracker;
            }

            if (dataStorage != null)
            {
                yield return dataStorage.GetBlogPostsTracker;
                yield return dataStorage.SaveBlogMetaTracker;
                yield return dataStorage.SaveChangesTracker;
            }
        }

        private static BlogSetting GetTestBlogSetting(string blogKey, string blogKeyName)
        {
            var blogSetting = new BlogSetting(blogKey, $"{blogKeyName}Id", $"{blogKeyName}Name");
            return blogSetting;
        }

        private static BlogSyncService GetTestSynchronizationService(
            IBlogSource blogSource = null,
            IDataStorage dataStorage = null)
        {
            blogSource = blogSource ?? MockBlogSource.Create();
            dataStorage = dataStorage ?? MockDataStorage.Create();

            var blogSettings = new[]
                                   {
                                       GetTestBlogSetting(BlogMetaTestData.BlogKey, nameof(BlogMetaTestData.BlogKey)),
                                       GetTestBlogSetting(BlogMetaTestData.BlogKey1, nameof(BlogMetaTestData.BlogKey1)),
                                       GetTestBlogSetting(BlogMetaTestData.BlogKey2, nameof(BlogMetaTestData.BlogKey2)),
                                       GetTestBlogSetting(BlogMetaTestData.BlogKey3, nameof(BlogMetaTestData.BlogKey3)),
                                   };

            var config = new BlogSyncConfiguration(
                             blogSource,
                             dataStorage,
                             BlogSyncConfigurationDefaults.BlavenIdProvider,
                             BlogSyncConfigurationDefaults.SlugProvider,
                             null,
                             blogSettings);

            var service = new BlogSyncService(config);
            return service;
        }
    }
}
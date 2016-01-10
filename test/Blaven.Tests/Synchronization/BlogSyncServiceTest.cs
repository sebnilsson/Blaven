using System;
using System.Collections.Generic;
using System.Linq;

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
        public void TryUpdate_SingleBlogKey_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce()
        {
            var service = GetTestSynchronizationService();

            service.TryUpdate(TestData.BlogKey);

            bool allTrackersHasRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(allTrackersHasRunOnce);
        }

        [Fact]
        public void TryUpdate_ParallelUsersAndSingleBlogKey_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce()
        {
            var service = GetTestSynchronizationService();

            service.TryUpdate(TestData.BlogKeys.ToArray());

            bool allTrackersHasRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(allTrackersHasRunOnce);
        }

        [Fact]
        public void
            TryUpdate_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorageDelegatesOncePerBlogKey()
        {
            var service = GetTestSynchronizationService();

            service.TryUpdate(TestData.BlogKeys.ToArray());

            bool allTrackersHasRunOncePerTestBlogKey =
                AllTrackers(tracker => HasRunTimesPerTestBlogKey(tracker, expectedCount: 1), service);

            Assert.True(allTrackersHasRunOncePerTestBlogKey);
        }

        [Fact]
        public void TryUpdate_ParallelUsersAndSingleBlogKey_ShouldNotRunAnyBlogSourceAndDataStorageDelegatesWithCollsion
            ()
        {
            var service = GetTestSynchronizationService();

            service.TryUpdate(TestData.BlogKey);

            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.False(anyTrackersWithCollision);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void
            TryUpdateInternal_RunMultipleTimesAndParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorageDelegatesOncePerTimeAndBlogKey
            (int additionalRunCount)
        {
            var service = GetTestSynchronizationService();

            var firstRunNow = new DateTime(2015, 1, 1, 12, 0, 0);

            TestUtility.RunParallelUsers(() => service.TryUpdateInternal(firstRunNow, TestData.BlogKeys));

            for (int i = 0; i < additionalRunCount; i++)
            {
                var additionalRunAddMinutes = (service.DataCacheHandler.TimeoutMinutes * (i + 1));
                var additionalRunAddSeconds = (10 * (i + 1));
                var additionalRunNow =
                    firstRunNow.AddMinutes(additionalRunAddMinutes).AddSeconds(additionalRunAddSeconds);

                TestUtility.RunParallelUsers(() => service.TryUpdateInternal(additionalRunNow, TestData.BlogKeys));
            }

            int expectedCount = (additionalRunCount + 1);

            bool allTrackersHasRunTwicePerTestBlogKey =
                AllTrackers(tracker => HasRunTimesPerTestBlogKey(tracker, expectedCount: expectedCount), service);

            Assert.True(allTrackersHasRunTwicePerTestBlogKey);
        }

        [Fact]
        public void TryUpdateInternal_RunTwiceWithinCacheTimeout_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce()
        {
            var service = GetTestSynchronizationService();

            var now1 = new DateTime(2015, 1, 1);
            var now2 = now1.AddMinutes(service.DataCacheHandler.TimeoutMinutes).AddMinutes(-1);

            var updatedBlogKeys1 = service.TryUpdateInternal(now1, TestData.BlogKey);
            var updatedBlogKeys2 = service.TryUpdateInternal(now2, TestData.BlogKey);

            bool allTrackersRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(updatedBlogKeys1.Contains(TestData.BlogKey));
            Assert.False(updatedBlogKeys2.Contains(TestData.BlogKey));
            Assert.True(allTrackersRunOnce);
        }

        [Fact]
        public void TryUpdateInternal_RunTwiceOutsideCacheTimeout_ShouldRunAllBlogSourceAndDataStorageDelegatesTwice()
        {
            var service = GetTestSynchronizationService();

            var now1 = new DateTime(2015, 1, 1);
            var now2 = now1.AddMinutes(service.DataCacheHandler.TimeoutMinutes).AddMinutes(1);

            var updatedBlogKeys1 = service.TryUpdateInternal(now1, TestData.BlogKey);
            var updatedBlogKeys2 = service.TryUpdateInternal(now2, TestData.BlogKey);

            bool allTrackersRunTwice = AllTrackers(tracker => tracker.RunCount == 2, service);

            Assert.True(updatedBlogKeys1.Contains(TestData.BlogKey));
            Assert.True(updatedBlogKeys2.Contains(TestData.BlogKey));
            Assert.True(allTrackersRunTwice);
        }

        [Fact]
        public void Update_ParallelUsersAndSingleBlogKey_ShouldRunWithCollsion()
        {
            var service = GetTestSynchronizationService();

            TestUtility.RunParallelUsers(() => service.Update(TestData.BlogKey));

            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.True(anyTrackersWithCollision);
        }

        [Fact]
        public void Update_ParallelUsersAndSingleBlogKey_ShouldRunAllBlogSourceAndDataStorage()
        {
            var service = GetTestSynchronizationService();

            TestUtility.RunParallelUsers(() => service.Update(TestData.BlogKey));

            bool allTrackersHasRunAll = AllTrackers(
                tracker => tracker.RunCount == TestUtility.ParallelUsersCount,
                service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public void Update_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorage()
        {
            var service = GetTestSynchronizationService();

            TestUtility.RunParallelUsers(() => service.Update(TestData.BlogKeys));

            int expectedRunCount = (TestUtility.ParallelUsersCount * TestData.BlogKeys.Length);

            bool allTrackersHasRunAll = AllTrackers(tracker => tracker.RunCount == expectedRunCount, service);

            Assert.True(allTrackersHasRunAll);
        }

        private static bool AllTrackers(Func<DelegateTracker<string>, bool> predicate, BlogSyncService service)
        {
            var trackers = GetServiceTrackers(service);

            bool allTrackers = AllTrackers(predicate, trackers);
            return allTrackers;
        }

        private static bool AllTrackers(
            Func<DelegateTracker<string>, bool> predicate,
            params DelegateTracker<string>[] trackers)
        {
            bool allTrackers = trackers.All(predicate);
            return allTrackers;
        }

        private static bool AnyTrackers(Func<DelegateTracker<string>, bool> predicate, BlogSyncService service)
        {
            var trackers = GetServiceTrackers(service);

            bool allTrackers = AnyTrackers(predicate, trackers);
            return allTrackers;
        }

        private static bool AnyTrackers(
            Func<DelegateTracker<string>, bool> predicate,
            params DelegateTracker<string>[] trackers)
        {
            bool allTrackers = trackers.Any(predicate);
            return allTrackers;
        }

        private static DelegateTracker<string>[] GetServiceTrackers(BlogSyncService service)
        {
            var trackers = GetServiceTrackersInternal(service).Where(x => x != null).ToArray();
            return trackers;
        }

        private static IEnumerable<DelegateTracker<string>> GetServiceTrackersInternal(BlogSyncService service)
        {
            var blogSource = service.BlogSource as MockBlogSource;
            var dataStorage = service.DataStorage as MockDataStorage;

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

        private static BlogSyncService GetTestSynchronizationService(
            IBlogSource blogSource = null,
            IDataStorage dataStorage = null)
        {
            blogSource = blogSource ?? MockBlogSource.Create();
            dataStorage = dataStorage ?? MockDataStorage.Create();

            var dataCacheHandler = new MemoryDataCacheHandler();

            var config = new BlogSyncConfiguration(
                blogSource,
                dataStorage,
                dataCacheHandler,
                BlogSyncConfigurationDefaults.BlavenIdProvider.Value,
                BlogSyncConfigurationDefaults.SlugProvider.Value);

            var service = new BlogSyncService(config);
            return service;
        }

        private static bool HasRunTimesPerTestBlogKey(DelegateTracker<string> tracker, int expectedCount = 1)
        {
            bool hasRunOncePerBlogKey =
                TestData.BlogKeys.All(
                    blogKey => tracker.KeyRunCount.ContainsKey(blogKey) && tracker.KeyRunCount[blogKey] == expectedCount);
            return hasRunOncePerBlogKey;
        }
    }
}
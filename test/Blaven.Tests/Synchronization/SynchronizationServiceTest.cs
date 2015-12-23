﻿using System;
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

            var actions = TestUtility.GetParallelActionsForSingleKey(() => service.TryUpdate(TestData.BlogKey));
            Parallel.Invoke(actions);

            bool allTrackersHasRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(allTrackersHasRunOnce);
        }

        [Fact]
        public void
            TryUpdate_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorageDelegatesOncePerBlogKey()
        {
            var service = GetTestSynchronizationService();

            var actions = TestUtility.GetParallelActionsForMultipleKeys(blogKey => service.TryUpdate(blogKey));
            Parallel.Invoke(actions);

            bool allTrackersHasRunOncePerTestBlogKey =
                AllTrackers(tracker => HasRunTimesPerTestBlogKey(tracker, expectedCount: 1), service);

            Assert.True(allTrackersHasRunOncePerTestBlogKey);
        }

        [Fact]
        public void TryUpdate_ParallelUsersAndSingleBlogKey_ShouldNotRunAnyBlogSourceAndDataStorageDelegatesWithCollsion
            ()
        {
            var service = GetTestSynchronizationService();

            var actions = TestUtility.GetParallelActionsForSingleKey(() => service.TryUpdate(TestData.BlogKey));
            Parallel.Invoke(actions);

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

            var firstRunActions =
                TestUtility.GetParallelActionsForMultipleKeys(
                    blogKey => service.TryUpdateInternal(blogKey, firstRunNow));
            Parallel.Invoke(firstRunActions);

            for (int i = 0; i < additionalRunCount; i++)
            {
                var additionalRunAddMinutes = (service.DataCacheHandler.TimeoutMinutes * (i + 1));
                var additionalRunAddSeconds = (10 * (i + 1));
                var additionalRunNow =
                    firstRunNow.AddMinutes(additionalRunAddMinutes).AddSeconds(additionalRunAddSeconds);

                var additionalRunActions =
                    TestUtility.GetParallelActionsForMultipleKeys(
                        blogKey => service.TryUpdateInternal(blogKey, additionalRunNow));

                Parallel.Invoke(additionalRunActions);
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

            bool hasUpdated1 = service.TryUpdateInternal(TestData.BlogKey, now1);
            bool hasUpdated2 = service.TryUpdateInternal(TestData.BlogKey, now2);

            bool allTrackersRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(hasUpdated1);
            Assert.False(hasUpdated2);
            Assert.True(allTrackersRunOnce);
        }

        [Fact]
        public void TryUpdateInternal_RunTwiceOutsideCacheTimeout_ShouldRunAllBlogSourceAndDataStorageDelegatesTwice()
        {
            var service = GetTestSynchronizationService();

            var now1 = new DateTime(2015, 1, 1);
            var now2 = now1.AddMinutes(service.DataCacheHandler.TimeoutMinutes).AddMinutes(1);

            bool hasUpdated1 = service.TryUpdateInternal(TestData.BlogKey, now1);
            bool hasUpdated2 = service.TryUpdateInternal(TestData.BlogKey, now2);

            bool allTrackersRunTwice = AllTrackers(tracker => tracker.RunCount == 2, service);

            Assert.True(hasUpdated1);
            Assert.True(hasUpdated2);
            Assert.True(allTrackersRunTwice);
        }

        [Fact]
        public void Update_ParallelUsersAndSingleBlogKey_ShouldRunWithCollsion()
        {
            var service = GetTestSynchronizationService();

            var actions = TestUtility.GetParallelActionsForSingleKey(() => service.Update(TestData.BlogKey));
            Parallel.Invoke(actions);

            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.True(anyTrackersWithCollision);
        }

        [Fact]
        public void Update_ParallelUsersAndSingleBlogKey_ShouldRunAllBlogSourceAndDataStorage()
        {
            var service = GetTestSynchronizationService();

            var actions = TestUtility.GetParallelActionsForSingleKey(() => service.Update(TestData.BlogKey));
            Parallel.Invoke(actions);

            bool allTrackersHasRunAll = AllTrackers(
                tracker => tracker.RunCount == TestUtility.SingleKeyUserCount,
                service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public void Update_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorage()
        {
            var service = GetTestSynchronizationService();

            var actions = TestUtility.GetParallelActionsForMultipleKeys(blogKey => service.Update(blogKey));
            Parallel.Invoke(actions);

            int expectedRunCount = (TestUtility.MultipleKeysUserCount * TestData.BlogKeys.Count);

            bool allTrackersHasRunAll = AllTrackers(tracker => tracker.RunCount == expectedRunCount, service);

            Assert.True(allTrackersHasRunAll);
        }

        private static bool AllTrackers(Func<DelegateTracker<string>, bool> predicate, SynchronizationService service)
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

        private static bool AnyTrackers(Func<DelegateTracker<string>, bool> predicate, SynchronizationService service)
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

        private static DelegateTracker<string>[] GetServiceTrackers(SynchronizationService service)
        {
            var trackers = GetServiceTrackersInternal(service).Where(x => x != null).ToArray();
            return trackers;
        }

        private static IEnumerable<DelegateTracker<string>> GetServiceTrackersInternal(SynchronizationService service)
        {
            var blogSource = service.BlogSource as TestBlogSource;
            var dataStorage = service.DataStorage as TestDataStorage;

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

        private static SynchronizationService GetTestSynchronizationService(
            IBlogSource blogSource = null,
            IDataStorage dataStorage = null)
        {
            blogSource = blogSource ?? TestBlogSource.Create();
            dataStorage = dataStorage ?? TestDataStorage.Create();

            var dataCacheHandler = new MemoryDataCacheHandler();

            var service = new SynchronizationService(blogSource, dataStorage, dataCacheHandler);
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
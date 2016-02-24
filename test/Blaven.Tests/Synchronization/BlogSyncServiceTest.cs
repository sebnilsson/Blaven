﻿using System;
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
        public void ForceUpdate_ParallelUsersAndSingleBlogKey_ShouldNotRunWithCollsion()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.ForceUpdate(TestData.BlogKey));

            // Assert
            bool anyTrackersWithCollision = AnyTrackers(
                tracker => tracker.KeyCollisionCount.Any(x => x.Value > 0),
                service);

            Assert.False(anyTrackersWithCollision);
        }

        [Fact]
        public void ForceUpdate_ParallelUsersAndSingleBlogKey_ShouldRunAllBlogSourceAndDataStorage()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.ForceUpdate(TestData.BlogKey));

            // Assert
            bool allTrackersHasRunAll = AllTrackers(
                tracker => tracker.RunCount == TestUtility.ParallelUsersCount,
                service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public void ForceUpdate_ParallelUsersAndMultipleBlogKeys_ShouldRunAllBlogSourceAndDataStorage()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.ForceUpdate(TestData.BlogKeys));

            // Assert
            int expectedRunCount = (TestUtility.ParallelUsersCount * TestData.BlogKeys.Length);

            bool allTrackersHasRunAll = AllTrackers(tracker => tracker.RunCount == expectedRunCount, service);

            Assert.True(allTrackersHasRunAll);
        }

        [Fact]
        public void TryUpdate_BlogKeyNotExisting_ShouldThrow()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act & Assert
            Assert.Throws<AggregateException>(() => service.TryUpdate("NON_EXISTING_BLOGKEY"));
        }

        [Fact]
        public void TryUpdate_SingleBlogKey_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            service.TryUpdate(TestData.BlogKey);

            // Assert
            bool allTrackersHasRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(allTrackersHasRunOnce);
        }

        [Theory]
        [MemberData(nameof(GetMultipleBlogKeys))]
        [MemberData(nameof(GetSingleBlogKey))]
        public void TryUpdate_MultipleOrSingleBlogKey_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce(
            string[] blogKeys)
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.TryUpdate(blogKeys));

            // Assert
            bool allTrackersHasRunOnce = AllTrackers(tracker => tracker.RunCount == blogKeys.Length, service);

            Assert.True(allTrackersHasRunOnce);
        }

        [Theory]
        [MemberData(nameof(GetMultipleBlogKeys))]
        [MemberData(nameof(GetSingleBlogKey))]
        public void TryUpdate_MultipleOrSingleBlogKey_ShouldRunAllBlogSourceAndDataStorageDelegatesOncePerBlogKey(
            string[] blogKeys)
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.TryUpdate(blogKeys));

            // Assert
            bool allTrackersHasRunOncePerTestBlogKey =
                AllTrackers(
                    tracker => HasRunTimesPerTestBlogKey(tracker, expectedCount: 1, blogKeys: blogKeys),
                    service);

            Assert.True(allTrackersHasRunOncePerTestBlogKey);
        }

        [Theory]
        [MemberData(nameof(GetMultipleBlogKeys))]
        [MemberData(nameof(GetSingleBlogKey))]
        public void TryUpdate_MultipleOrSingleBlogKey_ShouldNotRunWithAnyCollsion(string[] blogKeys)
        {
            // Arrange
            var service = GetTestSynchronizationService();

            // Act
            TestUtility.RunParallelUsers(() => service.TryUpdate(blogKeys));

            // Assert
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
            // Arrange
            var service = GetTestSynchronizationService();

            var firstRunNow = new DateTime(2015, 1, 1, 12, 0, 0);

            // Act
            TestUtility.RunParallelUsers(() => service.TryUpdateInternal(firstRunNow, TestData.BlogKeys));

            for (int i = 0; i < additionalRunCount; i++)
            {
                var additionalRunAddMinutes = (service.DataCacheHandler.TimeoutMinutes * (i + 1));
                var additionalRunAddSeconds = (10 * (i + 1));
                var additionalRunNow =
                    firstRunNow.AddMinutes(additionalRunAddMinutes).AddSeconds(additionalRunAddSeconds);

                TestUtility.RunParallelUsers(() => service.TryUpdateInternal(additionalRunNow, TestData.BlogKeys));
            }

            // Assert
            int expectedCount = (additionalRunCount + 1);

            bool allTrackersHasRunTwicePerTestBlogKey =
                AllTrackers(tracker => HasRunTimesPerTestBlogKey(tracker, expectedCount: expectedCount), service);

            Assert.True(allTrackersHasRunTwicePerTestBlogKey);
        }

        [Fact]
        public void TryUpdateInternal_RunTwiceWithinCacheTimeout_ShouldRunAllBlogSourceAndDataStorageDelegatesOnce()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            var now1 = new DateTime(2015, 1, 1);
            var now2 = now1.AddMinutes(service.DataCacheHandler.TimeoutMinutes).AddMinutes(-1);

            // Act
            var updatedBlogKeys1 = service.TryUpdateInternal(now1, TestData.BlogKey);
            var updatedBlogKeys2 = service.TryUpdateInternal(now2, TestData.BlogKey);

            // Assert
            bool updateBlogKeys1ContainsUpdatedBlogKey =
                updatedBlogKeys1.Any(x => x.BlogKey == TestData.BlogKey && x.IsUpdated);
            bool updateBlogKeys2ContainsUpdatedBlogKey =
                updatedBlogKeys2.Any(x => x.BlogKey == TestData.BlogKey && x.IsUpdated);
            bool allTrackersRunOnce = AllTrackers(tracker => tracker.RunCount == 1, service);

            Assert.True(updateBlogKeys1ContainsUpdatedBlogKey);
            Assert.False(updateBlogKeys2ContainsUpdatedBlogKey);
            Assert.True(allTrackersRunOnce);
        }

        [Fact]
        public void TryUpdateInternal_RunTwiceOutsideCacheTimeout_ShouldRunAllBlogSourceAndDataStorageDelegatesTwice()
        {
            // Arrange
            var service = GetTestSynchronizationService();

            var now1 = new DateTime(2015, 1, 1);
            var now2 = now1.AddMinutes(service.DataCacheHandler.TimeoutMinutes).AddMinutes(1);

            // Act
            var updatedBlogKeys1 = service.TryUpdateInternal(now1, TestData.BlogKey);
            var updatedBlogKeys2 = service.TryUpdateInternal(now2, TestData.BlogKey);

            // Assert
            bool updateBlogKeys1ContainsUpdatedBlogKey =
                updatedBlogKeys1.Any(x => x.BlogKey == TestData.BlogKey && x.IsUpdated);
            bool updateBlogKeys2ContainsUpdatedBlogKey =
                updatedBlogKeys2.Any(x => x.BlogKey == TestData.BlogKey && x.IsUpdated);
            bool allTrackersRunTwice = AllTrackers(tracker => tracker.RunCount == 2, service);

            Assert.True(updateBlogKeys1ContainsUpdatedBlogKey);
            Assert.True(updateBlogKeys2ContainsUpdatedBlogKey);
            Assert.True(allTrackersRunTwice);
        }

        public static IEnumerable<object[]> GetMultipleBlogKeys()
        {
            yield return new object[] { TestData.BlogKeys };
        }

        public static IEnumerable<object[]> GetSingleBlogKey()
        {
            yield return new object[] { new[] { TestData.BlogKey }.ToArray() };
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

            var dataCacheHandler = new MemoryDataCacheHandler();

            var blogSettings = new[]
                                   {
                                       GetTestBlogSetting(TestData.BlogKey, nameof(TestData.BlogKey)),
                                       GetTestBlogSetting(TestData.BlogKey1, nameof(TestData.BlogKey1)),
                                       GetTestBlogSetting(TestData.BlogKey2, nameof(TestData.BlogKey2)),
                                       GetTestBlogSetting(TestData.BlogKey3, nameof(TestData.BlogKey3)),
                                   };

            var config = new BlogSyncConfiguration(
                blogSource,
                dataStorage,
                dataCacheHandler,
                BlogSyncConfigurationDefaults.BlavenIdProvider,
                BlogSyncConfigurationDefaults.SlugProvider,
                null,
                blogSettings);

            var service = new BlogSyncService(config);
            return service;
        }

        private static bool HasRunTimesPerTestBlogKey(
            DelegateTracker<BlogSetting> tracker,
            int expectedCount = 1,
            params string[] blogKeys)
        {
            if (blogKeys == null || !blogKeys.Any())
            {
                blogKeys = TestData.BlogKeys;
            }

            bool hasRunOncePerBlogKey = blogKeys.All(
                blogKey =>
                    {
                        var trackerKvp = tracker.KeyRunCount.FirstOrDefault(x => x.Key.BlogKey == blogKey);

                        bool hasRunExpectedCount = (trackerKvp.Key != null && trackerKvp.Value == expectedCount);
                        return hasRunExpectedCount;
                    });
            return hasRunOncePerBlogKey;
        }
    }
}
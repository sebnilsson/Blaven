using System;

using Blaven.Tests;
using Xunit;

namespace Blaven.Data.Tests
{
    public class MemoryDataCacheHandlerTest
    {
        private static readonly DateTime NowFirst = new DateTime(2015, 1, 1);

        private static readonly DateTime NowSecond = new DateTime(2015, 3, 1);

        [Fact]
        public void ctor_WithNegativeNumber_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MemoryDataCacheHandler(-1));
        }

        [Fact]
        public void ctor_WithZero_ShouldThrow()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MemoryDataCacheHandler(0));
        }

        [Fact]
        public void OnUpdated_WithNoDataStates_ShouldAddEntry()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            cacheHandler.OnUpdated(TestData.BlogKey, NowFirst);

            var dataState = cacheHandler.DataUpdatedAt[TestData.BlogKey];
            Assert.Equal(NowFirst, dataState);
        }

        [Fact]
        public void OnUpdated_WithMultipleKeysAndNoDataStates_ShouldAddEntries()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];
            Assert.Equal(NowFirst, dataState1);
            Assert.Equal(NowFirst, dataState2);
        }

        [Fact]
        public void OnUpdated_WithExistingDataStates_ShouldUpdateEntry()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            cacheHandler.OnUpdated(TestData.BlogKey, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey, NowSecond);

            var dataState = cacheHandler.DataUpdatedAt[TestData.BlogKey];
            Assert.Equal(NowSecond, dataState);
        }

        [Fact]
        public void OnUpdated_WithMultipleKeysAndExistingDataStates_ShouldUpdateEntries()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey1, NowSecond);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowSecond);

            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];
            Assert.Equal(NowSecond, dataState1);
            Assert.Equal(NowSecond, dataState2);
        }

        [Fact]
        public void OnUpdated_WithMultipleKeysAndOneExistingDataStates_ShouldUpdateOneEntry()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey2, NowSecond);

            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];
            Assert.Equal(NowFirst, dataState1);
            Assert.Equal(NowSecond, dataState2);
        }

        [Fact]
        public void IsUpdated_WithNoDataStates_ShouldReturnFalse()
        {
            var cacheHandler = new MemoryDataCacheHandler();

            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, DateTime.MinValue);

            Assert.False(isUpdated);
        }

        [Fact]
        public void IsUpdated_WithDataStatesInsideLimit_ShouldReturnTrue()
        {
            var cacheHandler = new MemoryDataCacheHandler();
            cacheHandler.DataUpdatedAt[TestData.BlogKey] = NowFirst;

            var now = NowFirst.AddMinutes(MemoryDataCacheHandler.DefaultTimeoutMinutes);

            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, now);

            Assert.True(isUpdated);
        }

        [Fact]
        public void IsUpdated_WithDataStatesOutsideLimit_ShouldReturnFalse()
        {
            var cacheHandler = new MemoryDataCacheHandler();
            cacheHandler.DataUpdatedAt[TestData.BlogKey] = NowFirst;

            var now = NowFirst.AddMinutes(MemoryDataCacheHandler.DefaultTimeoutMinutes).AddSeconds(1);

            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, now);

            Assert.False(isUpdated);
        }
    }
}
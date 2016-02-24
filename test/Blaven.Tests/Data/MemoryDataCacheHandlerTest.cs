﻿using System;

using Blaven.Tests;
using Xunit;

namespace Blaven.Data.Tests
{
    public class MemoryDataCacheHandlerTest
    {
        private static readonly DateTime NowFirst = new DateTime(2015, 1, 1);

        private static readonly DateTime NowSecond = new DateTime(2015, 3, 1);

        [Fact]
        public void ctor_NegativeNumber_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new MemoryDataCacheHandler(-1));
        }

        [Fact]
        public void ctor_Zero_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new MemoryDataCacheHandler(0));
        }

        [Fact]
        public void OnUpdated_NoDataStates_ShouldAddEntry()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            cacheHandler.OnUpdated(TestData.BlogKey, NowFirst);

            // Assert
            var dataState = cacheHandler.DataUpdatedAt[TestData.BlogKey];

            Assert.Equal(NowFirst, dataState);
        }

        [Fact]
        public void OnUpdated_MultipleKeysAndNoDataStates_ShouldAddEntries()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            // Assert
            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];

            Assert.Equal(NowFirst, dataState1);
            Assert.Equal(NowFirst, dataState2);
        }

        [Fact]
        public void OnUpdated_ExistingDataStates_ShouldUpdateEntry()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            cacheHandler.OnUpdated(TestData.BlogKey, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey, NowSecond);

            // Assert
            var dataState = cacheHandler.DataUpdatedAt[TestData.BlogKey];
            Assert.Equal(NowSecond, dataState);
        }

        [Fact]
        public void OnUpdated_MultipleKeysAndExistingDataStates_ShouldUpdateEntries()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey1, NowSecond);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowSecond);

            // Assert
            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];

            Assert.Equal(NowSecond, dataState1);
            Assert.Equal(NowSecond, dataState2);
        }

        [Fact]
        public void OnUpdated_MultipleKeysAndOneExistingDataStates_ShouldUpdateOneEntry()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            cacheHandler.OnUpdated(TestData.BlogKey1, NowFirst);
            cacheHandler.OnUpdated(TestData.BlogKey2, NowFirst);

            cacheHandler.OnUpdated(TestData.BlogKey2, NowSecond);

            // Assert
            var dataState1 = cacheHandler.DataUpdatedAt[TestData.BlogKey1];
            var dataState2 = cacheHandler.DataUpdatedAt[TestData.BlogKey2];

            Assert.Equal(NowFirst, dataState1);
            Assert.Equal(NowSecond, dataState2);
        }

        [Fact]
        public void IsUpdated_NoDataStates_ShouldReturnFalse()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();

            // Act
            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, DateTime.MinValue);

            // Assert
            Assert.False(isUpdated);
        }

        [Fact]
        public void IsUpdated_DataStatesInsideLimit_ShouldReturnTrue()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();
            cacheHandler.DataUpdatedAt[TestData.BlogKey] = NowFirst;

            var now = NowFirst.AddMinutes(MemoryDataCacheHandler.DefaultTimeoutMinutes);

            // Act
            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, now);

            // Assert
            Assert.True(isUpdated);
        }

        [Fact]
        public void IsUpdated_DataStatesOutsideLimit_ShouldReturnFalse()
        {
            // Arrange
            var cacheHandler = new MemoryDataCacheHandler();
            cacheHandler.DataUpdatedAt[TestData.BlogKey] = NowFirst;

            var now = NowFirst.AddMinutes(MemoryDataCacheHandler.DefaultTimeoutMinutes).AddSeconds(1);

            // Act
            bool isUpdated = cacheHandler.IsUpdated(TestData.BlogKey, now);

            // Assert
            Assert.False(isUpdated);
        }
    }
}
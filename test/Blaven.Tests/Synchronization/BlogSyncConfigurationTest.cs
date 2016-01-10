using System;

using Blaven.BlogSources.Tests;
using Blaven.Data;
using Blaven.Data.Tests;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class SynchronizationConfigurationTest
    {
        [Fact]
        public void Ctor_BlogSourceNullArgument_ThrowsArgumentNullException()
        {
            var dataStorage = new MockDataStorage();

            Assert.Throws<ArgumentNullException>(
                () => new BlogSyncConfiguration(null, dataStorage, null, null, null));
        }

        [Fact]
        public void Ctor_DataStorageNullArgument_ThrowsArgumentNullException()
        {
            var blogSource = new MockBlogSource();

            Assert.Throws<ArgumentNullException>(
                () => new BlogSyncConfiguration(blogSource, null, null, null, null));
        }

        [Fact]
        public void BlavenIdProvider_CtorNullArgument_ReturnsDefaultType()
        {
            var blogSource = new MockBlogSource();
            var dataStorage = new MockDataStorage();

            var config = new BlogSyncConfiguration(blogSource, dataStorage, null, null, null);

            Assert.NotNull(config.BlavenIdProvider);
            Assert.IsType<BlavenBlogPostBlavenIdProvider>(config.BlavenIdProvider);
        }

        [Fact]
        public void DataCacheHandler_CtorNullArgument_ReturnsDefaultType()
        {
            var blogSource = new MockBlogSource();
            var dataStorage = new MockDataStorage();

            var config = new BlogSyncConfiguration(blogSource, dataStorage, null, null, null);

            Assert.NotNull(config.DataCacheHandler);
            Assert.IsType<MemoryDataCacheHandler>(config.DataCacheHandler);
        }

        [Fact]
        public void SlugProvider_CtorNullArgument_ReturnsDefaultType()
        {
            var blogSource = new MockBlogSource();
            var dataStorage = new MockDataStorage();

            var config = new BlogSyncConfiguration(blogSource, dataStorage, null, null, null);

            Assert.NotNull(config.SlugProvider);
            Assert.IsType<BlavenBlogPostUrlSlugProvider>(config.SlugProvider);
        }
    }
}
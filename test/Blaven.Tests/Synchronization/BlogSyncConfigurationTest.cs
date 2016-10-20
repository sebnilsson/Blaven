using System;

using Blaven.BlogSources;
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
            // Arrange
            var dataStorage = new MockDataStorage();
            
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GetTestBlogSyncConfiguration(null, dataStorage));
        }

        [Fact]
        public void Ctor_DataStorageNullArgument_ThrowsArgumentNullException()
        {
            // Arrange
            var blogSource = new MockBlogSource();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => GetTestBlogSyncConfiguration(blogSource));
        }

        [Fact]
        public void BlavenIdProvider_CtorNullArgument_ReturnsDefaultType()
        {
            // Arrange
            var blogSource = new MockBlogSource();
            var dataStorage = new MockDataStorage();

            // Act
            var config = GetTestBlogSyncConfiguration(blogSource, dataStorage);

            // Assert
            Assert.NotNull(config.BlavenIdProvider);
            Assert.IsType<PermalinkBlogPostBlavenIdProvider>(config.BlavenIdProvider);
        }

        [Fact]
        public void SlugProvider_CtorNullArgument_ReturnsDefaultType()
        {
            // Arrange
            var blogSource = new MockBlogSource();
            var dataStorage = new MockDataStorage();

            // Act
            var config = GetTestBlogSyncConfiguration(blogSource, dataStorage);

            // Assert
            Assert.NotNull(config.SlugProvider);
            Assert.IsType<BlavenBlogPostUrlSlugProvider>(config.SlugProvider);
        }

        private static BlogSyncConfiguration GetTestBlogSyncConfiguration(
            IBlogSource blogSource = null,
            IDataStorage dataStorage = null)
        {
            var config = new BlogSyncConfiguration(blogSource, dataStorage, null, null, null, null);
            return config;
        }
    }
}
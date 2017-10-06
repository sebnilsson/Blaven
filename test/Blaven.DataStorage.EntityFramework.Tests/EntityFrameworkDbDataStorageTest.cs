using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Testing;
using Xunit;

namespace Blaven.DataStorage.EntityFramework.Tests
{
    using Blaven.DataStorage.Testing;

    public class EntityFrameworkDataStorageTest : DataStorageTestBase
    {
        [Fact]
        public override async Task GetBlogPosts_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            // Arrange & Act & Assert
            await base.GetBlogPosts_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts();
        }

        [Fact]
        public override async Task GetBlogPosts_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            // Arrange & Act & Assert
            await base.GetBlogPosts_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts();
        }

        [Fact]
        public override async Task<IDataStorage> SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta() as EntityFrameworkDataStorage;

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DbContext.BlogMetas.FirstOrDefault(x => x.BlogKey == BlogMetaTestData.BlogKey);
            Assert.NotNull(ravenDbBlogMeta);

            return dataStorage;
        }

        [Fact]
        public override async Task<IDataStorage> SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta() as EntityFrameworkDataStorage;

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DbContext.BlogMetas.FirstOrDefault(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(UpdatedBlogMeta.Description, ravenDbBlogMeta.Description);
            Assert.Equal(UpdatedBlogMeta.Name, ravenDbBlogMeta.Name);
            Assert.Equal(UpdatedBlogMeta.PublishedAt, ravenDbBlogMeta.PublishedAt);
            Assert.Equal(UpdatedBlogMeta.Url, ravenDbBlogMeta.Url);
            Assert.Equal(UpdatedBlogMeta.UpdatedAt, ravenDbBlogMeta.UpdatedAt);

            return dataStorage;
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public override async Task<IDataStorage> SaveChanges_DeletedPosts_DbContainsRemainingPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_DeletedPosts_DbContainsRemainingPosts(dbBlogPosts) as EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);
            Assert.Equal(2, blogPostCount);

            return dataStorage;
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public override async Task<IDataStorage> SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(dbBlogPosts) as
                    EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(8, blogPostCount);

            return dataStorage;
        }


        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public override async Task<IDataStorage> SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(dbBlogPosts) as
                    EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(8, blogPostCount);

            return dataStorage;
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public override async Task<IDataStorage> SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(dbBlogPosts)
                    as EntityFrameworkDataStorage;

            // Assert
            int blogPostCount =
                dataStorage.DbContext.BlogPosts.Count(
                    x => x.BlogKey.Equals(BlogMetaTestData.BlogKey, StringComparison.OrdinalIgnoreCase));

            Assert.Equal(11, blogPostCount);

            return dataStorage;
        }

        protected override IDataStorage GetDataStorage(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null)
        {
            var dbContext = BlavenDbContextTestFactory.CreateWithData(blogMetas, blogPosts);

            var dataStorage = new EntityFrameworkDataStorage(dbContext);
            return dataStorage;
        }
    }
}
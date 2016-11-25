using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataStorage.Tests;
using Blaven.Tests;
using Xunit;

namespace Blaven.DataStorage.EntityFramework.Tests
{
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
        public new async Task SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta() as EntityFrameworkDataStorage;

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DbContext.BlogMetas.FirstOrDefault(x => x.BlogKey == BlogMetaTestData.BlogKey);
            Assert.NotNull(ravenDbBlogMeta);
        }

        [Fact]
        public new async Task SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
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
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public new async Task SaveChanges_DeletedPosts_DbContainsRemainingPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_DeletedPosts_DbContainsRemainingPosts(dbBlogPosts) as EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);
            Assert.Equal(2, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public new async Task SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(dbBlogPosts) as
                    EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(8, blogPostCount);
        }


        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public new async Task SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act
            var dataStorage =
                await base.SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(dbBlogPosts) as
                    EntityFrameworkDataStorage;

            // Assert
            int blogPostCount = dataStorage.DbContext.BlogPosts.Count(x => x.BlogKey == BlogMetaTestData.BlogKey);

            Assert.Equal(8, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public new async Task SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
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
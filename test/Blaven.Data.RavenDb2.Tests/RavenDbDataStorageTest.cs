using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Tests;
using Xunit;

namespace Blaven.Data.RavenDb2.Tests
{
    public class RavenDbDataStorageTest
    {
        private static BlogMeta UpdatedBlogMeta
            =>
            new BlogMeta
                {
                    BlogKey = BlogMetaTestData.BlogKey,
                    Description = "Updated-BlogMeta_Description",
                    Name = "Updated-BlogMeta_Name",
                    PublishedAt = BlogMetaTestData.BlogMetaPublishedAt.AddDays(15),
                    Url = "Updated-BlogMeta_Url",
                    UpdatedAt = BlogMetaTestData.BlogMetaUpdatedAt.AddDays(30)
                };

        [Fact]
        public async Task GetBlogPosts_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            // Arrange
            var dbBlogPosts1 = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: BlogMetaTestData.BlogKey1);
            var dbBlogPosts2 = BlogPostTestData.CreateCollection(start: 2, count: 2, blogKey: BlogMetaTestData.BlogKey2);
            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts1.Concat(dbBlogPosts2));
            var blogSetting2 = BlogSettingTestData.Create(BlogMetaTestData.BlogKey2);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting2);

            // Assert
            bool allPostsHasBlogKey2 = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey2);
            Assert.True(allPostsHasBlogKey2);
        }

        [Fact]
        public async Task GetBlogPosts_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            // Arrange
            const int PostCount = 2500;

            var dbBlogPosts = BlogPostTestData.CreateCollection(start: 0, count: PostCount).ToList();
            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting);

            // Assert
            Assert.Equal(PostCount, posts.Count);
        }

        [Fact]
        public async Task SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange
            var dataStorage = GetRavenDbDataStorage();
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var blogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, blogMeta);

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DocumentStore.QueryNonStale<BlogMeta, BlogMeta>(
                    query => query.FirstOrDefault(x => x.BlogKey == BlogMetaTestData.BlogKey));
            Assert.NotNull(ravenDbBlogMeta);
        }

        [Fact]
        public async Task SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            // Arrange
            var dbBlogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);
            var dataStorage = GetRavenDbDataStorage(blogMetas: new[] { dbBlogMeta });
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var updatedBlogMeta = UpdatedBlogMeta;

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, updatedBlogMeta);

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DocumentStore.QueryNonStale<BlogMeta, BlogMeta>(
                    query => query.FirstOrDefault(x => x.BlogKey == BlogMetaTestData.BlogKey));

            Assert.Equal(UpdatedBlogMeta.Description, ravenDbBlogMeta.Description);
            Assert.Equal(UpdatedBlogMeta.Name, ravenDbBlogMeta.Name);
            Assert.Equal(UpdatedBlogMeta.PublishedAt, ravenDbBlogMeta.PublishedAt);
            Assert.Equal(UpdatedBlogMeta.Url, ravenDbBlogMeta.Url);
            Assert.Equal(UpdatedBlogMeta.UpdatedAt, ravenDbBlogMeta.UpdatedAt);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task SaveChanges_DeletedPosts_DbContainsRemainingPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var deletedBlogPosts = BlogPostTestData.CreateCollection(start: 2, count: 3);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(deletedBlogPosts: deletedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == BlogMetaTestData.BlogKey));
            Assert.Equal(2, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(insertedBlogPosts: insertedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == BlogMetaTestData.BlogKey));
            Assert.Equal(8, blogPostCount);
        }


        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var updatedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(updatedBlogPosts: updatedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == BlogMetaTestData.BlogKey));
            Assert.Equal(8, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5,
             MemberType = typeof(BlogPostTheoryData))]
        public async Task SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);
            var updatedBlogPosts = BlogPostTestData.CreateCollection(start: 6, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(
                insertedBlogPosts: insertedBlogPosts,
                updatedBlogPosts: updatedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == BlogMetaTestData.BlogKey));
            Assert.Equal(11, blogPostCount);
        }

        private static RavenDbDataStorage GetRavenDbDataStorage(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null)
        {
            var documentStore = EmbeddableDocumentStoreHelper.GetWithData(blogMetas: blogMetas, blogPosts: blogPosts);

            var dataStorage = new RavenDbDataStorage(documentStore);
            return dataStorage;
        }
    }
}
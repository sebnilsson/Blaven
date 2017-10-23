using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Synchronization.Testing;
using Blaven.Testing;
using Xunit;

namespace Blaven.DataStorage.Testing
{
    public abstract class DataStorageTestBase
    {
        protected static BlogMeta UpdatedBlogMeta => new BlogMeta
                                                     {
                                                         BlogKey = BlogMetaTestData.BlogKey,
                                                         Description = "Updated-BlogMeta_Description",
                                                         Name = "Updated-BlogMeta_Name",
                                                         PublishedAt =
                                                             BlogMetaTestData.BlogMetaPublishedAt.AddDays(
                                                                 15),
                                                         Url = "Updated-BlogMeta_Url",
                                                         UpdatedAt = BlogMetaTestData.BlogMetaUpdatedAt
                                                             .AddDays(30)
                                                     };

        public virtual async Task GetBlogPosts_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            // Arrange
            const int postCount = 2500;

            var dbBlogPosts = BlogPostTestData.CreateCollection(0, postCount).ToList();
            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting, null);

            // Assert
            Assert.Equal(postCount, posts.Count);
        }

        public virtual async Task GetBlogPosts_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            // Arrange
            var dbBlogPosts1 = BlogPostTestData.CreateCollection(0, 2, BlogMetaTestData.BlogKey1);
            var dbBlogPosts2 = BlogPostTestData.CreateCollection(2, 2, BlogMetaTestData.BlogKey2);
            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts1.Concat(dbBlogPosts2));
            var blogSetting2 = BlogSettingTestData.Create(BlogMetaTestData.BlogKey2);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting2, null);

            // Assert
            var allPostsHasBlogKey2 = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey2);

            Assert.True(allPostsHasBlogKey2);
        }

        public virtual async Task<IDataStorage> SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            // Arrange
            var dbBlogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);
            var dataStorage = GetDataStorage(new[] { dbBlogMeta });
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var updatedBlogMeta = UpdatedBlogMeta;

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, updatedBlogMeta);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange
            var dataStorage = GetDataStorage();
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var blogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, blogMeta);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveChanges_DeletedPosts_DbContainsRemainingPosts(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var deletedBlogPosts = BlogPostTestData.CreateCollection(2, 3);

            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(deletedBlogPosts: deletedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        public virtual async Task<IDataStorage>
            SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
                IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(3, 5);
            var updatedBlogPosts = BlogPostTestData.CreateCollection(6, 5);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(
                insertedBlogPosts: insertedBlogPosts,
                updatedBlogPosts: updatedBlogPosts);

            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(3, 5);

            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(insertedBlogPosts: insertedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var updatedBlogPosts = BlogPostTestData.CreateCollection(3, 5);

            var dataStorage = GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(updatedBlogPosts: updatedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        protected abstract IDataStorage GetDataStorage(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null);
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Synchronization.Tests;
using Blaven.Tests;
using Xunit;

namespace Blaven.DataStorage.Tests
{
    public abstract class DataStorageTestBase
    {
        protected static BlogMeta UpdatedBlogMeta
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

        public virtual async Task GetBlogPosts_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            // Arrange
            var dbBlogPosts1 = BlogPostTestData.CreateCollection(start: 0, count: 2, blogKey: BlogMetaTestData.BlogKey1);
            var dbBlogPosts2 = BlogPostTestData.CreateCollection(start: 2, count: 2, blogKey: BlogMetaTestData.BlogKey2);
            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts1.Concat(dbBlogPosts2));
            var blogSetting2 = BlogSettingTestData.Create(BlogMetaTestData.BlogKey2);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting2, lastUpdatedAt: null);

            // Assert
            bool allPostsHasBlogKey2 = posts.Any() && posts.All(x => x.BlogKey == BlogMetaTestData.BlogKey2);
            Assert.True(allPostsHasBlogKey2);
        }

        public virtual async Task GetBlogPosts_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            // Arrange
            const int PostCount = 2500;

            var dbBlogPosts = BlogPostTestData.CreateCollection(start: 0, count: PostCount).ToList();
            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            var posts = await dataStorage.GetBlogPosts(blogSetting, lastUpdatedAt: null);

            // Assert
            Assert.Equal(PostCount, posts.Count);
        }

        public virtual async Task<IDataStorage> SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange
            var dataStorage = this.GetDataStorage();
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var blogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, blogMeta);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            // Arrange
            var dbBlogMeta = BlogMetaTestData.Create(BlogMetaTestData.BlogKey);
            var dataStorage = this.GetDataStorage(blogMetas: new[] { dbBlogMeta });
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var updatedBlogMeta = UpdatedBlogMeta;

            // Act
            await dataStorage.SaveBlogMeta(blogSetting, updatedBlogMeta);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveChanges_DeletedPosts_DbContainsRemainingPosts(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var deletedBlogPosts = BlogPostTestData.CreateCollection(start: 2, count: 3);

            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(deletedBlogPosts: deletedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        public virtual async Task<IDataStorage> SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);

            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts);
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
            var updatedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);

            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(updatedBlogPosts: updatedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        public virtual async Task<IDataStorage>
            SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
                IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = BlogPostTestData.CreateCollection(start: 3, count: 5);
            var updatedBlogPosts = BlogPostTestData.CreateCollection(start: 6, count: 5);

            var dataStorage = this.GetDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = BlogSettingTestData.Create(BlogMetaTestData.BlogKey);
            var changeSet = BlogSyncChangeSetTestData.CreateWithData(
                insertedBlogPosts: insertedBlogPosts,
                updatedBlogPosts: updatedBlogPosts);

            // Act
            await dataStorage.SaveChanges(blogSetting, changeSet);

            return dataStorage;
        }

        protected abstract IDataStorage GetDataStorage(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null);
    }
}
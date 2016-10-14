using System;
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
                        BlogKey = TestData.BlogKey,
                        Description = "Updated-BlogMeta_Description",
                        Name = "Updated-BlogMeta_Name",
                        PublishedAt = TestData.BlogMetaPublishedAt.AddDays(15),
                        Url = "Updated-BlogMeta_Url",
                        UpdatedAt = TestData.BlogMetaUpdatedAt.AddDays(30)
                    };

        [Fact]
        public async Task GetPostBases_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            // Arrange
            var dbBlogPosts1 = TestData.GetBlogPosts(start: 0, count: 2, blogKey: TestData.BlogKey1);
            var dbBlogPosts2 = TestData.GetBlogPosts(start: 2, count: 2, blogKey: TestData.BlogKey2);
            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts1.Concat(dbBlogPosts2));
            var blogSetting2 = TestData.GetBlogSetting(TestData.BlogKey2);

            // Act
            var posts = await dataStorage.GetPostBases(blogSetting2);

            // Assert
            bool allPostsHasBlogKey2 = posts.Any() && posts.All(x => x.BlogKey == TestData.BlogKey2);
            Assert.True(allPostsHasBlogKey2);
        }

        [Fact]
        public async Task GetPostBases_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            // Arrange
            const int PostCount = 2500;

            var dbBlogPosts = TestData.GetBlogPosts(start: 0, count: PostCount).ToList();
            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);

            // Act
            var posts = await dataStorage.GetPostBases(blogSetting);

            // Assert
             Assert.Equal(PostCount, posts.Count);
        }

        [Fact]
        public void SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            // Arrange
            var dataStorage = GetRavenDbDataStorage();
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var blogMeta = TestData.GetBlogMeta(TestData.BlogKey);

            // Act
            dataStorage.SaveBlogMeta(blogSetting, blogMeta);

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DocumentStore.QueryNonStale<BlogMeta, BlogMeta>(
                    query => query.FirstOrDefault(x => x.BlogKey == TestData.BlogKey));
            Assert.NotNull(ravenDbBlogMeta);
        }

        [Fact]
        public void SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            // Arrange
            var dbBlogMeta = TestData.GetBlogMeta(TestData.BlogKey);
            var dataStorage = GetRavenDbDataStorage(blogMetas: new[] { dbBlogMeta });
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var updatedBlogMeta = UpdatedBlogMeta;

            // Act
            dataStorage.SaveBlogMeta(blogSetting, updatedBlogMeta);

            // Assert
            var ravenDbBlogMeta =
                dataStorage.DocumentStore.QueryNonStale<BlogMeta, BlogMeta>(
                    query => query.FirstOrDefault(x => x.BlogKey == TestData.BlogKey));

            Assert.Equal(UpdatedBlogMeta.Description, ravenDbBlogMeta.Description);
            Assert.Equal(UpdatedBlogMeta.Name, ravenDbBlogMeta.Name);
            Assert.Equal(UpdatedBlogMeta.PublishedAt, ravenDbBlogMeta.PublishedAt);
            Assert.Equal(UpdatedBlogMeta.Url, ravenDbBlogMeta.Url);
            Assert.Equal(UpdatedBlogMeta.UpdatedAt, ravenDbBlogMeta.UpdatedAt);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5, MemberType = typeof(TestData))]
        public void SaveChanges_DeletedPosts_DbContainsRemainingPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var deletedBlogPosts = TestData.GetBlogPosts(start: 2, count: 3);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(deletedBlogPosts: deletedBlogPosts);

            // Act
            dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == TestData.BlogKey));
            Assert.Equal(2, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5, MemberType = typeof(TestData))]
        public void SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = TestData.GetBlogPosts(start: 3, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(insertedBlogPosts: insertedBlogPosts);

            // Act
            dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == TestData.BlogKey));
            Assert.Equal(8, blogPostCount);
        }


        [Theory]
        [MemberData(nameof(TestData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5, MemberType = typeof(TestData))]
        public void SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var updatedBlogPosts = TestData.GetBlogPosts(start: 3, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(updatedBlogPosts: updatedBlogPosts);

            // Act
            dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == TestData.BlogKey));
            Assert.Equal(8, blogPostCount);
        }

        [Theory]
        [MemberData(nameof(TestData.GetDbBlogPostsForSingleAndMultipleKeys), 0, 5, MemberType = typeof(TestData))]
        public void SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange
            var insertedBlogPosts = TestData.GetBlogPosts(start: 3, count: 5);
            var updatedBlogPosts = TestData.GetBlogPosts(start: 6, count: 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: dbBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(
                insertedBlogPosts: insertedBlogPosts,
                updatedBlogPosts: updatedBlogPosts);

            // Act
            dataStorage.SaveChanges(blogSetting, changeSet);

            // Assert
            int blogPostCount =
                dataStorage.DocumentStore.QueryNonStale<BlogPost, int>(
                    query => query.Count(x => x.BlogKey == TestData.BlogKey));
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
using System.Collections.Generic;
using System.Linq;

using Blaven.BlogSources;
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
        public void GetPostBases_SettingWithBlogKey2_ReturnsOnlyBlogKey2Posts()
        {
            var blogPosts1 = TestData.GetBlogPosts(0, 2, TestData.BlogKey1);
            var blogPosts2 = TestData.GetBlogPosts(2, 2, TestData.BlogKey2);
            var dataStorage = GetRavenDbDataStorage(blogPosts: blogPosts1.Concat(blogPosts2));
            var blogSetting2 = TestData.GetBlogSetting(TestData.BlogKey2);

            var posts = dataStorage.GetPostBases(blogSetting2);

            bool allPostsHasBlogKey2 = posts.Any() && posts.All(x => x.BlogKey == TestData.BlogKey2);
            Assert.True(allPostsHasBlogKey2);
        }

        [Fact]
        public void GetPostBases_PostCountMoreThanRavenDbPageCount_ReturnsAllPosts()
        {
            const int PostCount = 2500;

            var blogPosts = TestData.GetBlogPosts(0, PostCount);
            var dataStorage = GetRavenDbDataStorage(blogPosts: blogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);

            var posts = dataStorage.GetPostBases(blogSetting);

            Assert.Equal(PostCount, posts.Count);
        }

        [Fact]
        public void SaveBlogMeta_NonExistingBlogMeta_ReturnsNewBlogMeta()
        {
            var dataStorage = GetRavenDbDataStorage();
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var blogMeta = TestData.GetBlogMeta(TestData.BlogKey);

            dataStorage.SaveBlogMeta(blogSetting, blogMeta);

            BlogMeta ravenDbBlogMeta;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                ravenDbBlogMeta = session.QueryNonStale<BlogMeta>().FirstOrDefault(x => x.BlogKey == TestData.BlogKey);
            }

            Assert.NotNull(ravenDbBlogMeta);
        }

        [Fact]
        public void SaveBlogMeta_ExistingBlogMeta_ReturnsUpdatedBlogMeta()
        {
            var existingBlogMeta = TestData.GetBlogMeta(TestData.BlogKey);
            var dataStorage = GetRavenDbDataStorage(blogMetas: new[] { existingBlogMeta });
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var updatedBlogMeta = UpdatedBlogMeta;

            dataStorage.SaveBlogMeta(blogSetting, updatedBlogMeta);

            BlogMeta ravenDbBlogMeta;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                ravenDbBlogMeta = session.QueryNonStale<BlogMeta>().FirstOrDefault(x => x.BlogKey == TestData.BlogKey);
            }

            Assert.Equal(UpdatedBlogMeta.Description, ravenDbBlogMeta.Description);
            Assert.Equal(UpdatedBlogMeta.Name, ravenDbBlogMeta.Name);
            Assert.Equal(UpdatedBlogMeta.PublishedAt, ravenDbBlogMeta.PublishedAt);
            Assert.Equal(UpdatedBlogMeta.Url, ravenDbBlogMeta.Url);
            Assert.Equal(UpdatedBlogMeta.UpdatedAt, ravenDbBlogMeta.UpdatedAt);
        }

        [Fact]
        public void SaveChanges_DeletedPosts_DbContainsRemainingPosts()
        {
            var existingBlogPosts = TestData.GetBlogPosts(0, 5);
            var deletedBlogPosts = TestData.GetBlogPosts(2, 3);

            var dataStorage = GetRavenDbDataStorage(blogPosts: existingBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(deletedBlogPosts: deletedBlogPosts);

            dataStorage.SaveChanges(blogSetting, changeSet);

            int blogPostCount;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                blogPostCount = session.QueryNonStale<BlogPost>().Count(x => x.BlogKey == TestData.BlogKey);
            }

            Assert.Equal(2, blogPostCount);
        }

        [Fact]
        public void SaveChanges_InsertedPostsOverlapping_DbContainsPostsWithoutDuplicates()
        {
            var existingBlogPosts = TestData.GetBlogPosts(0, 5);
            var insertedBlogPosts = TestData.GetBlogPosts(3, 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: existingBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(insertedBlogPosts: insertedBlogPosts);

            dataStorage.SaveChanges(blogSetting, changeSet);

            int blogPostCount;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                blogPostCount = session.QueryNonStale<BlogPost>().Count(x => x.BlogKey == TestData.BlogKey);
            }

            Assert.Equal(8, blogPostCount);
        }

        [Fact]
        public void SaveChanges_UpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates()
        {
            var existingBlogPosts = TestData.GetBlogPosts(0, 5);
            var updatedBlogPosts = TestData.GetBlogPosts(3, 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: existingBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(updatedBlogPosts: updatedBlogPosts);

            dataStorage.SaveChanges(blogSetting, changeSet);

            int blogPostCount;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                blogPostCount = session.QueryNonStale<BlogPost>().Count(x => x.BlogKey == TestData.BlogKey);
            }

            Assert.Equal(8, blogPostCount);
        }

        [Fact]
        public void SaveChanges_InsertedAndUpdatedPostsOverlapping_DbContainsPostsWithoutDuplicates()
        {
            var existingBlogPosts = TestData.GetBlogPosts(0, 5);
            var insertedBlogPosts = TestData.GetBlogPosts(3, 5);
            var updatedBlogPosts = TestData.GetBlogPosts(6, 5);

            var dataStorage = GetRavenDbDataStorage(blogPosts: existingBlogPosts);
            var blogSetting = TestData.GetBlogSetting(TestData.BlogKey);
            var changeSet = TestData.GetBlogSourceChangeSetWithData(
                insertedBlogPosts: insertedBlogPosts,
                updatedBlogPosts: updatedBlogPosts);

            dataStorage.SaveChanges(blogSetting, changeSet);

            int blogPostCount;
            using (var session = dataStorage.DocumentStore.OpenSession())
            {
                blogPostCount = session.QueryNonStale<BlogPost>().Count(x => x.BlogKey == TestData.BlogKey);
            }

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
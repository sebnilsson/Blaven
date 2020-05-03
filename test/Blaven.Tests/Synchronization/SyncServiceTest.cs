using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Testing;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class SyncServiceTest
    {
        protected readonly ServicesContext ServicesContext;

        public SyncServiceTest()
        {
            ServicesContext = new ServicesContext();
        }

        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            ServicesContext.ConfigBlogSource(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            ServicesContext.ConfigStorageSyncRepo(
                BlogPostTestFactory.CreateList(2, 3));

            var syncService = ServicesContext.GetSyncService();

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Equal(2, result.Posts.Inserted.Count);
            Assert.Empty(result.Posts.Updated);
            Assert.Empty(result.Posts.Deleted);
        }

        [Theory]
        [MemberData(nameof(GetChangedBlogPostData))]
        public async Task Synchronize_ContainsHashUpdates_ReturnsUpdates(
            BlogPost[] blogSourcePosts,
            int expectedUpdateCount)
        {
            // Arrange
            ServicesContext.ConfigBlogSource(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            ServicesContext.ConfigStorageSyncRepo(blogSourcePosts);

            var syncService = ServicesContext.GetSyncService();

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Equal(expectedUpdateCount, result.Posts.Updated.Count);
            Assert.Empty(result.Posts.Inserted);
            Assert.Empty(result.Posts.Deleted);
        }

        [Fact]
        public async Task Synchronize_ContainsDeleted_ReturnsDeleted()
        {
            // Arrange
            ServicesContext.ConfigBlogSource(
                BlogPostTestFactory.CreateList(2, 3));

            ServicesContext.ConfigStorageSyncRepo(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            var syncService = ServicesContext.GetSyncService();

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Equal(2, result.Posts.Deleted.Count);
            Assert.Empty(result.Posts.Inserted);
            Assert.Empty(result.Posts.Updated);
        }

        [Fact]
        public async Task Synchronize_NoChanges_ReturnsNoChanges()
        {
            // Arrange
            ServicesContext.ConfigBlogSource(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            ServicesContext.ConfigStorageSyncRepo(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            var syncService = ServicesContext.GetSyncService();

            // Act
            var result = await syncService.Synchronize();

            // Assert
            Assert.Empty(result.Posts.Inserted);
            Assert.Empty(result.Posts.Updated);
            Assert.Empty(result.Posts.Deleted);
        }

        public static IEnumerable<object[]> GetChangedBlogPostData()
        {
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Hash = "CHANGED_VALUE"; }),
                2
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Author.Name = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Author.ImageUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Author.Name = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Author.Url = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Content = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.ImageUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Slug = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.SourceUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Summary = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                GetChangedBlogPosts(x => { x.Title = "CHANGED_VALUE"; }),
                0
            };
        }

        private static BlogPost[] GetChangedBlogPosts(
            Action<BlogPost> config)
        {
            return
                new BlogPost[]
                {
                    BlogPostTestFactory.Create(1),
                    BlogPostTestFactory.Create(2, config),
                    BlogPostTestFactory.Create(3, config),
                    BlogPostTestFactory.Create(4)
                };
        }
    }
}

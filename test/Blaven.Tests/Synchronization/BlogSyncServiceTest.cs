using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.InMemory;
using Blaven.Testing;
using Blaven.Transformation;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class BlogSyncServiceTest
    {
        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
            var storagePosts = BlogPostTestFactory.CreateList(2, 3);

            var syncService = GetSyncService(blogSourcePosts, storagePosts);

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
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetSyncService(blogSourcePosts, storagePosts);

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
            var blogSourcePosts = BlogPostTestFactory.CreateList(2, 3);
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetSyncService(blogSourcePosts, storagePosts);

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
            var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
            var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

            var syncService = GetSyncService(blogSourcePosts, storagePosts);

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
                CreateChangedBlogPosts(x => { x.Hash = "CHANGED_VALUE"; }),
                2
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Author.Name = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Author.ImageUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Author.Name = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Author.Url = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Content = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.ImageUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Slug = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.SourceUrl = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Summary = "CHANGED_VALUE"; }),
                0
            };
            yield return new object[]
            {
                CreateChangedBlogPosts(x => { x.Title = "CHANGED_VALUE"; }),
                0
            };
        }

        private static BlogPost[] CreateChangedBlogPosts(
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

        private IBlogSyncService GetSyncService(
            IReadOnlyList<BlogPost>? blogSourcePosts = null,
            IReadOnlyList<BlogPost>? storagePosts = null)
        {
            var blogSource = new FakeBlogSource(blogSourcePosts);

            var inMemoryStorage = new InMemoryStorage(
                Enumerable.Empty<BlogMeta>(),
                storagePosts ?? Enumerable.Empty<BlogPost>());

            var storageSyncRepo =
                new InMemoryStorageSyncRepository(inMemoryStorage);

            var transformService =
                new BlogPostStorageTransformService(
                    Enumerable.Empty<IBlogPostStorageTransform>());

            return new BlogSyncService(
                blogSource,
                storageSyncRepo,
                transformService);
        }
    }
}

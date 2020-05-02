using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSource;
using Blaven.DependencyInjection;
using Blaven.Storage;
using Blaven.Synchronization;
using Blaven.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Blaven.EndToEndTests.Synchronization
{
    public class SyncServiceTest
    {
        private readonly Lazy<IServiceProvider> _services;
        private readonly IServiceCollection _serviceCollection;
        private readonly InMemoryStorageRepository _inMemoryStorage;

        public SyncServiceTest()
        {
            _serviceCollection = new ServiceCollection();
            _serviceCollection.AddBlaven();

            _inMemoryStorage = new InMemoryStorageRepository();

            _serviceCollection.AddSingleton<IStorageSyncRepository>(_inMemoryStorage);

            _services = new Lazy<IServiceProvider>(() =>
                _serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
            var storagePosts = BlogPostTestFactory.CreateList(2, 3);

            var service =
                GetSynchronizationService(blogSourcePosts, storagePosts);

            // Act
            await service.Synchronize();

            // Assert
            var blogService =
                _services.Value.GetRequiredService<IBlogService>();

            var posts = await blogService.ListPostHeaders();

            Assert.Equal(2, posts.Count);
        }

        //[Theory]
        //[MemberData(nameof(GetChangedBlogPostData))]
        //public async Task Synchronize_ContainsHashUpdates_ReturnsUpdates(
        //    BlogPost[] blogSourcePosts,
        //    int expectedUpdateCount)
        //{
        //    // Arrange
        //    var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

        //    var service =
        //        GetSynchronizationService(blogSourcePosts, storagePosts);

        //    // Act
        //    var result = await service.Synchronize();

        //    // Assert
        //    Assert.Equal(expectedUpdateCount, result.Posts.Updated.Count);
        //    Assert.Empty(result.Posts.Inserted);
        //    Assert.Empty(result.Posts.Deleted);
        //}

        //[Fact]
        //public async Task Synchronize_ContainsDeleted_ReturnsDeleted()
        //{
        //    // Arrange
        //    var blogSourcePosts = BlogPostTestFactory.CreateList(2, 3);
        //    var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

        //    var service =
        //        GetSynchronizationService(blogSourcePosts, storagePosts);

        //    // Act
        //    var result = await service.Synchronize();

        //    // Assert
        //    Assert.Equal(2, result.Posts.Deleted.Count);
        //    Assert.Empty(result.Posts.Inserted);
        //    Assert.Empty(result.Posts.Updated);
        //}

        //[Fact]
        //public async Task Synchronize_NoChanges_ReturnsNoChanges()
        //{
        //    // Arrange
        //    var blogSourcePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);
        //    var storagePosts = BlogPostTestFactory.CreateList(1, 2, 3, 4);

        //    var service =
        //        GetSynchronizationService(blogSourcePosts, storagePosts);

        //    // Act
        //    var result = await service.Synchronize();

        //    // Assert
        //    Assert.Empty(result.Posts.Inserted);
        //    Assert.Empty(result.Posts.Updated);
        //    Assert.Empty(result.Posts.Deleted);
        //}

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

        private ISyncService GetSynchronizationService(
            IEnumerable<BlogPost> blogSourcePosts,
            IEnumerable<BlogPost> storagePosts)
        {
            var blogSource = new Mock<IBlogSource>();
            blogSource.Setup(x =>
                x.GetPosts(It.IsAny<BlogKey>(), It.IsAny<DateTimeOffset?>()))
            .Returns(Task.FromResult(
                blogSourcePosts.ToList() as IReadOnlyList<BlogPost>));

            _serviceCollection.AddTransient(_ => blogSource.Object);

            _inMemoryStorage.Posts.AddRange(storagePosts);

            return
                _services.Value.GetRequiredService<ISyncService>();
        }
    }
}

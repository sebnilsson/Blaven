using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSources;
using Blaven.BlogSources.FileProviders;
using Blaven.BlogSources.Markdown;
using Blaven.DependencyInjection;
using Blaven.Synchronization;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Blaven.MarkdownFilesEndToEndTests
{
    public class BlogQueryServiceTest
    {
        [Fact]
        public async Task GetMeta_ExistingMeta_ReturnMeta()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var meta = await blogQueryService.GetMeta();

            // Assert
            Assert.NotNull(meta);
            Assert.Equal("Single Blog Description", meta?.Description);
            Assert.Equal("Single Blog", meta?.Name);
            Assert.Equal("https://single-blog.com", meta?.Url);
        }

        [Fact]
        public async Task GetPostBySlug_ExistingPost_ReturnPost()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var post = await blogQueryService.GetPostBySlug("test-blog-post");

            // Assert
            var expectedTags =
                Enumerable.Range(1, 3).Select(x => $"Test Tag {x}").ToList();

            Assert.NotNull(post);
            Assert.Equal("Test Post Title", post?.Title);
            Assert.Equal("Test Post Summary", post?.Summary);
            Assert.Equal("test-blog-post", post?.Slug);
            Assert.Equal("Test Author Name", post?.Author?.Name);
            Assert.Equal("https://test.com/author/image.png", post?.Author?.ImageUrl);
            Assert.Equal("https://test.com/author", post?.Author?.Url);
            Assert.Equal(new DateTime(2020, 1, 2, 3, 4, 5), post?.PublishedAt);
            Assert.Equal(new DateTime(2020, 2, 3, 4, 5, 6), post?.UpdatedAt);

            var tagsSequenceEquals =
                expectedTags.SequenceEqual(post?.Tags);

            Assert.True(tagsSequenceEquals);
        }

        [Fact]
        public async Task ListAllDates_ExistingPost_ReturnDates()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var dates = await blogQueryService.ListAllDates();

            // Assert
            var expectedDates = new[]
            {
                new BlogDateItem
                {
                    Date = new DateTime(2020, 3, 1),
                    Count = 2
                },
                new BlogDateItem
                {
                    Date = new DateTime(2020, 1, 1),
                    Count = 1
                }
            };

            var sequenceEquals =
                expectedDates.All(x => dates.Any(y =>
                    y.BlogKey == x.BlogKey
                    && y.Count == x.Count
                    && y.Date == x.Date));

            Assert.True(sequenceEquals);
        }

        [Fact]
        public async Task ListAllMetas_ExistingMetas_ReturnMetas()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var metas = await blogQueryService.ListAllMetas();

            // Assert
            Assert.NotNull(metas);
            Assert.NotEmpty(metas);
        }

        [Fact]
        public async Task ListAllTags_ExistingTags_ReturnTags()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var tags = await blogQueryService.ListAllTags();

            // Assert
            var expectedTags = new[]
            {
                new BlogTagItem
                {
                    Name = "Test Tag 1",
                    Count = 1
                },
                new BlogTagItem
                {
                    Name = "Test Tag 2",
                    Count = 3
                },
                new BlogTagItem
                {
                    Name = "Test Tag 3",
                    Count = 3
                },
                new BlogTagItem
                {
                    Name = "Test Tag 4",
                    Count = 2
                }
            };

            var sequenceEquals =
                expectedTags.All(x => tags.Any(y =>
                    y.BlogKey == x.BlogKey
                    && y.Count == x.Count
                    && y.Name == x.Name));

            Assert.True(sequenceEquals);
        }

        [Fact]
        public async Task ListPosts_ExistingPosts_ReturnPosts()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var posts = await blogQueryService.ListPosts();

            // Assert
            Assert.NotNull(posts);
            Assert.Equal(3, posts.Count);
        }

        [Fact]
        public async Task ListPostsByArchive_ExistingPosts_ReturnPosts()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var archiveDates = new DateTime(2020, 3, 1);
            var posts = await blogQueryService.ListPostsByArchive(archiveDates);

            // Assert
            Assert.NotNull(posts);
            Assert.Equal(2, posts.Count);
        }

        [Fact]
        public async Task ListPostsByTag_ExistingPosts_ReturnPosts()
        {
            // Arrange
            var blogQueryService = await GetBlogQueryService();

            // Act
            var posts = await blogQueryService.ListPostsByTag("Test Tag 3");

            // Assert
            Assert.NotNull(posts);
            Assert.Equal(3, posts.Count);
        }

        private async Task<IBlogQueryService> GetBlogQueryService()
        {
            var services = GetServiceProvider();

            var blogSyncService =
                services.GetRequiredService<IBlogSyncService>();

            await blogSyncService.Synchronize();

            return services.GetRequiredService<IBlogQueryService>();
        }

        private static IServiceProvider GetServiceProvider(
            string filePath = "DiskResources/single-blog")
        {
            var services = new ServiceCollection();

            services.AddBlaven();
            services.AddBlavenInMemoryStorage();

            services.AddTransient<IBlogSource, MarkdownBlogSource>();

            services.AddTransient<IFileDataProvider>(x =>
            {
                var baseDirectory =
                    Path.Combine(
                        Environment.CurrentDirectory,
                        filePath);

                return
                    new DiskFileDataProvider(
                        baseDirectory,
                        recursive: true,
                        metaExtensions: new[] { ".json" },
                        postExtensions: new[] { ".md" });
            });

            return services.BuildServiceProvider();
        }
    }
}

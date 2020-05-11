using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.InMemory;
using Blaven.Testing;
using Blaven.Transformation;
using Xunit;

namespace Blaven.Tests
{
    public class BlogQueryServiceTest
    {
        private const string BlogKey1 = "BLOG_KEY_1";
        private const string BlogKey2 = "BLOG_KEY_2";

        [Fact]
        public async Task ListPosts_NoBlogKeyAndNoPaging_ReturnsAllPosts()
        {
            // Arrange
            var storagePosts = GetBlogPosts();

            var blogQueryService = GetBlogQueryServices(storagePosts);

            // Act
            var posts = await blogQueryService.ListPosts();

            // Assert
            Assert.Equal(10, posts.Count);
        }

        [Fact]
        public async Task ListPosts_BlogKeyAndNoPaging_ReturnsPostsWithBlogKey()
        {
            // Arrange
            var storagePosts = GetBlogPosts();

            var blogQueryService = GetBlogQueryServices(storagePosts);

            // Act
            var posts =
                await blogQueryService.ListPosts(blogKeys: BlogKey1);

            // Assert
            var allPostsHasBlogKey =
                posts.All(x => x.BlogKey == BlogKey1);

            Assert.Equal(4, posts.Count);
            Assert.True(allPostsHasBlogKey);
        }

        [Fact]
        public async Task ListPosts_BlogKeyAndPaging_ReturnsPagedPostsWithBlogKey()
        {
            // Arrange
            var storagePosts = GetBlogPosts();

            var blogQueryService = GetBlogQueryServices(storagePosts);

            var paging1 = new Paging(pageIndex: 0, pageSize: 2);
            var paging2 = new Paging(pageIndex: 1, pageSize: 2);

            // Act
            var posts1 =
                await blogQueryService.ListPosts(paging1, BlogKey1);
            var posts2 =
                await blogQueryService.ListPosts(paging2, BlogKey1);

            // Assert
            var allPostsHasBlogKey1 =
                posts1.All(x => x.BlogKey == BlogKey1);
            var allPostsHasBlogKey2 =
                posts2.All(x => x.BlogKey == BlogKey1);
            var pagesAreDifferent =
                !posts1.Any(x => posts2.Contains(x));

            Assert.Equal(2, posts1.Count);
            Assert.Equal(2, posts2.Count);
            Assert.True(allPostsHasBlogKey1);
            Assert.True(allPostsHasBlogKey2);
            Assert.True(pagesAreDifferent);
        }

        [Fact]
        public async Task GetPost_ValidIdAndValidBlogKey_ReturnsPost()
        {
            // Arrange
            var storagePosts = GetBlogPosts();

            var blogQueryService = GetBlogQueryServices(storagePosts);

            // Act
            var post =
                await blogQueryService.GetPost("2", BlogKey1);

            // Assert
            Assert.NotNull(post);
        }

        [Fact]
        public async Task GetPost_ValidIdAndInvalidBlogKey_ReturnsNull()
        {
            // Arrange
            var storagePosts = GetBlogPosts();

            var blogQueryService = GetBlogQueryServices(storagePosts);

            // Act
            var post =
                await blogQueryService.GetPost("2", BlogKey2);

            // Assert
            Assert.Null(post);
        }

        private static IReadOnlyList<BlogPost> GetBlogPosts()
        {
            return new List<BlogPost>
            {
                BlogPostTestFactory.Create(1, BlogKey1),
                BlogPostTestFactory.Create(2, BlogKey1),
                BlogPostTestFactory.Create(3, BlogKey1),
                BlogPostTestFactory.Create(4, BlogKey1),
                BlogPostTestFactory.Create(5, BlogKey2),
                BlogPostTestFactory.Create(6, BlogKey2),
                BlogPostTestFactory.Create(7, BlogKey2),
                BlogPostTestFactory.Create(8, "BLOG_KEY_3"),
                BlogPostTestFactory.Create(9, "BLOG_KEY_3"),
                BlogPostTestFactory.Create(10, "BLOG_KEY_3"),
            };
        }

        private IBlogQueryService GetBlogQueryServices(
            IReadOnlyList<BlogPost>? storagePosts = null)
        {
            var inMemoryStorage = new InMemoryStorage(
                Enumerable.Empty<BlogMeta>(),
                storagePosts ?? Enumerable.Empty<BlogPost>());

            var storageQueryRepo =
                new InMemoryStorageQueryRepository(inMemoryStorage);

            var queryTransformService =
                new BlogPostQueryTransformService(
                    Enumerable.Empty<IBlogPostQueryTransform>());

            return
                new BlogQueryService(
                    storageQueryRepo,
                    queryTransformService);
        }
    }
}

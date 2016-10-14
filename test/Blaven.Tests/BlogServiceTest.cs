using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Data.Tests;
using Xunit;

namespace Blaven.Tests
{
    public class BlogServiceTest
    {
        [Fact]
        public void ctor_NullBlogSettings_ShouldNotThrow()
        {
            // Arrange
            var repository = new MockRepository();

            // Act
            var service = new BlogService(repository, blogSettings: null);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void ctor_NullRepository_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BlogService(repository: null));
        }

        [Fact]
        public async Task GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta()
        {
            // Arrange
            var blogMetas = new[] { new BlogMeta { BlogKey = TestData.BlogKey, Name = TestData.BlogMetaName } };
            var service = GetBlogService(blogMetas: blogMetas);

            // Act
            var meta = await service.GetBlogMeta(TestData.BlogKey);

            // Assert
            Assert.Equal(TestData.BlogKey, meta.BlogKey);
            Assert.Equal(TestData.BlogMetaName, meta.Name);
        }

        [Fact]
        public async Task GetBlogMeta_EmptyExistingBlogKey_ReturnsFirstBlogMeta()
        {
            // Arrange
            var blogMetas = new[]
                                {
                                    new BlogMeta { BlogKey = TestData.BlogKey1, Name = TestData.BlogMetaName },
                                    new BlogMeta { BlogKey = TestData.BlogKey2, Name = $"{TestData.BlogMetaName}_Name" }
                                };
            var service = GetBlogService(blogMetas: blogMetas);

            // Act
            var meta = await service.GetBlogMeta();

            // Assert
            Assert.Equal(TestData.BlogKeys.First(), meta.BlogKey);
            Assert.Equal(TestData.BlogMetaName, meta.Name);
        }

        [Fact]
        public async Task GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var meta = await service.GetBlogMeta($"NonExisting_{TestData.BlogKey}");

            // Assert
            Assert.Null(meta);
        }

        [Fact]
        public async Task GetPost_ExistingBlogKey_ReturnsBlogPost()
        {
            // Arrange
            var mockBlogPost = TestData.GetBlogPost(TestData.BlogKey);
            var service = GetBlogService(blogPosts: new[] { mockBlogPost });

            // Act
            var post = await service.GetPost(mockBlogPost.BlavenId, TestData.BlogKey);

            // Assert
            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public async Task GetPost_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var post = await service.GetPost($"NonExisting_{TestData.BlogKey}", string.Empty);

            // Assert
            Assert.Null(post);
        }

        [Fact]
        public async Task GetPostBySourceId_ExistingBlogKey_ReturnsBlogPost()
        {
            // Arrange
            var mockBlogPost = TestData.GetBlogPost(TestData.BlogKey);
            var service = GetBlogService(blogPosts: new[] { mockBlogPost });

            // Act
            var post = await service.GetPostBySourceId(mockBlogPost.SourceId, TestData.BlogKey);

            // Assert
            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public async Task GetPostBySourceId_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var post = await service.GetPostBySourceId($"NonExisting_{TestData.BlogKey}", string.Empty);

            // Assert
            Assert.Null(post);
        }

        [Fact]
        public void ListArchive_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogService(mockBlogPosts);

            // Act
            var archive = service.ListArchive(TestData.BlogKey).ToList();

            // Assert
            bool archiveContainsAllMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListArchive_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(35, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var archive = service.ListArchive(TestData.BlogKey2).ToList();

            // Assert
            var firstArchiveItem = archive.FirstOrDefault();

            bool archiveContainsBlogKeyMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts2);

            Assert.Equal(mockBlogPosts1.Count, firstArchiveItem?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListArchive_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(35, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var archive = service.ListArchive(TestData.BlogKey1, TestData.BlogKey2).ToList();

            // Assert
            var firstArchiveItem = archive.FirstOrDefault();
            var secondArchiveItem = archive.ElementAtOrDefault(1);

            bool archiveContainsAllMockPosts = GetArchiveAllContainsBlogPosts(archive, allMockBlogPosts);

            Assert.Equal(mockBlogPosts1.Count, firstArchiveItem?.Count);
            Assert.Equal(mockBlogPosts2.Count, secondArchiveItem?.Count);
            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListArchive_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var archive = service.ListArchive($"NonExisting_{TestData.BlogKey}");

            // Assert
            Assert.False(archive.Any());
        }

        [Fact]
        public void ListTags_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogService(mockBlogPosts);

            // Act
            var tags = service.ListTags(TestData.BlogKey).ToList();

            // Assert
            bool tagsContainsAllMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListTags_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var tags = service.ListTags(TestData.BlogKey1).ToList();

            // Assert
            var tag5 = tags.FirstOrDefault(x => x.Name.EndsWith(" 5"));

            bool archiveContainsBlogKeyMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts1);

            Assert.Equal(5, tag5?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListTags_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var tags = service.ListTags(TestData.BlogKey1, TestData.BlogKey2).ToList();

            // Assert
            var tag5 = tags.FirstOrDefault(x => x.Name.EndsWith(" 5"));
            var tag11 = tags.FirstOrDefault(x => x.Name.EndsWith(" 11"));

            bool archiveContainsAllMockPosts = GetTagsAllContainsBlogPostTags(tags, allMockBlogPosts);

            Assert.Equal(5, tag5?.Count);
            Assert.Equal(4, tag11?.Count);
            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListTags_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var tags = service.ListTags($"NonExisting_{TestData.BlogKey}");

            // Assert
            Assert.False(tags.Any());
        }

        [Fact]
        public void ListPostHeads_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogService(mockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(TestData.BlogKey).ToList();

            // Assert
            bool tagsContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(TestData.BlogKey1).ToList();

            // Assert
            bool archiveContainsBlogKeyMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPostHeads_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(TestData.BlogKey1, TestData.BlogKey2).ToList();

            // Assert
            bool archiveContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var postHeads = service.ListPostHeads($"NonExisting_{TestData.BlogKey}");

            // Assert
            Assert.False(postHeads.Any());
        }

        [Fact]
        public void ListPosts_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogService(mockBlogPosts);

            // Act
            var posts = service.ListPosts(TestData.BlogKey).ToList();

            // Assert
            bool tagsContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var posts = service.ListPosts(TestData.BlogKey1).ToList();

            // Assert
            bool archiveContainsBlogKeyMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPosts_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogService(allMockBlogPosts);

            // Act
            var posts = service.ListPosts(TestData.BlogKey1, TestData.BlogKey2).ToList();

            // Assert
            bool archiveContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = GetBlogService();

            // Act
            var posts = service.ListPosts($"NonExisting_{TestData.BlogKey}");

            // Assert
            Assert.False(posts.Any());
        }

        private static bool GetArchiveAllContainsBlogPosts(
            ICollection<BlogArchiveItem> archive,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !archive.Any())
            {
                return false;
            }

            bool archiveAllContainsBlogPosts =
                blogPosts.All(
                    p =>
                    p.PublishedAt.HasValue
                    && archive.Any(
                        a => p.PublishedAt.Value.Year == a.Date.Year && p.PublishedAt.Value.Month == a.Date.Month));
            return archiveAllContainsBlogPosts;
        }

        private static BlogService GetBlogService(
            IEnumerable<BlogPost> blogPosts = null,
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<string> blogSettingBlogKeys = null,
            int funcSleep = 100)
        {
            var blogSettings =
                (blogSettingBlogKeys ?? TestData.BlogKeys).Select(x => new BlogSetting(x, $"{x}_Id", $"{x}_Name"))
                    .ToArray();

            var repository = MockRepository.Create(blogPosts, blogMetas, funcSleep);

            var service = new BlogService(repository, blogSettings);
            return service;
        }

        private static bool GetPostHeadsAllContainsBlogs(
            ICollection<BlogPostHead> postHeads,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !postHeads.Any())
            {
                return false;
            }

            bool postHeadsAllContainsBlogPosts =
                blogPosts.All(
                    p =>
                    postHeads.Any(
                        h =>
                        h.BlogKey == p.BlogKey && h.BlavenId == p.BlavenId && h.Hash == p.Hash && h.Title == p.Title));
            return postHeadsAllContainsBlogPosts;
        }

        private static bool GetPostsAllContainsBlogPosts(
            ICollection<BlogPost> blogPosts1,
            IEnumerable<BlogPost> blogPosts2,
            bool expectAny = true)
        {
            if (expectAny && !blogPosts1.Any())
            {
                return false;
            }

            bool postHeadsAllContainsBlogPosts =
                blogPosts2.All(
                    p2 =>
                    blogPosts1.Any(
                        p1 =>
                        p1.BlogKey == p2.BlogKey && p1.BlavenId == p2.BlavenId && p1.Hash == p2.Hash
                        && p1.Title == p2.Title));
            return postHeadsAllContainsBlogPosts;
        }

        private static bool GetTagsAllContainsBlogPostTags(
            ICollection<BlogTagItem> tags,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !tags.Any())
            {
                return false;
            }

            bool tagsAllContainsBlogPosts = blogPosts.All(p => tags.Any(t => p.Tags.Contains(t.Name)));
            return tagsAllContainsBlogPosts;
        }
    }
}
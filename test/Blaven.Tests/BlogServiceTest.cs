﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.DataStorage.Testing;
using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class BlogServiceTest
    {
        [Fact]
        public void ctor_NullBlogSettings_ShouldNotThrow()
        {
            // Arrange
            var repository = new FakeRepository();

            // Act
            var service = new BlogService(repository, null);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void ctor_NullRepository_ShouldThrow()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new BlogService(null));
        }

        [Fact]
        public async Task GetBlogMeta_EmptyExistingBlogKey_ReturnsFirstBlogMeta()
        {
            // Arrange
            var blogMetaName2 = $"{BlogMetaTestData.BlogMetaName}_2";

            var service = BlogServiceTestFactory.Create(
                blogMetas: new[]
                           {
                               new BlogMeta
                               {
                                   BlogKey = BlogMetaTestData.BlogKey2,
                                   Name = blogMetaName2
                               },
                               new BlogMeta
                               {
                                   BlogKey = BlogMetaTestData.BlogKey1,
                                   Name = BlogMetaTestData.BlogMetaName
                               }
                           },
                blogSettings: new[]
                              {
                                  BlogSettingTestData.Create(BlogMetaTestData.BlogKey2),
                                  BlogSettingTestData.Create(BlogMetaTestData.BlogKey1)
                              });

            // Act
            var meta = await service.GetBlogMeta();

            // Assert
            Assert.Equal(BlogMetaTestData.BlogKey2, meta.BlogKey);
            Assert.Equal(blogMetaName2, meta.Name);
        }

        [Fact]
        public async Task GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta()
        {
            // Arrange
            var blogMetas = new[]
                            {
                                new BlogMeta
                                {
                                    BlogKey = BlogMetaTestData.BlogKey,
                                    Name = BlogMetaTestData.BlogMetaName
                                }
                            };
            var service = BlogServiceTestFactory.Create(blogMetas: blogMetas);

            // Act
            var meta = await service.GetBlogMeta(BlogMetaTestData.BlogKey);

            // Assert
            Assert.Equal(BlogMetaTestData.BlogKey, meta.BlogKey);
            Assert.Equal(BlogMetaTestData.BlogMetaName, meta.Name);
        }

        [Fact]
        public async Task GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var meta = await service.GetBlogMeta($"NonExisting_{BlogMetaTestData.BlogKey}");

            // Assert
            Assert.Null(meta);
        }

        [Fact]
        public async Task GetPost_ExistingBlogKey_ReturnsBlogPost()
        {
            // Arrange
            var mockBlogPost = BlogPostTestData.Create();
            var service = BlogServiceTestFactory.Create(new[] { mockBlogPost });

            // Act
            var post = await service.GetPost(mockBlogPost.BlavenId, BlogMetaTestData.BlogKey);

            // Assert
            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public async Task GetPost_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var post = await service.GetPost($"NonExisting_{BlogMetaTestData.BlogKey}", string.Empty);

            // Assert
            Assert.Null(post);
        }

        [Fact]
        public async Task GetPostBySourceId_ExistingBlogKey_ReturnsBlogPost()
        {
            // Arrange
            var mockBlogPost = BlogPostTestData.Create();
            var service = BlogServiceTestFactory.Create(new[] { mockBlogPost });

            // Act
            var post = await service.GetPostBySourceId(mockBlogPost.SourceId, BlogMetaTestData.BlogKey);

            // Assert
            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public async Task GetPostBySourceId_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var post = await service.GetPostBySourceId($"NonExisting_{BlogMetaTestData.BlogKey}", string.Empty);

            // Assert
            Assert.Null(post);
        }

        [Fact]
        public void ListArchive_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(35, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var archive = service.ListArchive(BlogMetaTestData.BlogKey1, BlogMetaTestData.BlogKey2).ToList();

            // Assert
            var firstArchiveItem = archive.FirstOrDefault();
            var secondArchiveItem = archive.ElementAtOrDefault(1);

            var archiveContainsAllMockPosts = GetArchiveAllContainsBlogPosts(archive, allMockBlogPosts);

            Assert.Equal(mockBlogPosts1.Count, firstArchiveItem?.Count);
            Assert.Equal(mockBlogPosts2.Count, secondArchiveItem?.Count);
            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListArchive_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey).ToList();
            var service = BlogServiceTestFactory.Create(mockBlogPosts);

            // Act
            var archive = service.ListArchive(BlogMetaTestData.BlogKey).ToList();

            // Assert
            var archiveContainsAllMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListArchive_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var archive = service.ListArchive($"NonExisting_{BlogMetaTestData.BlogKey}");

            // Assert
            Assert.False(archive.Any());
        }

        [Fact]
        public void ListArchive_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(35, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var archive = service.ListArchive(BlogMetaTestData.BlogKey2).ToList();

            // Assert
            var firstArchiveItem = archive.FirstOrDefault();

            var archiveContainsBlogKeyMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts2);

            Assert.Equal(mockBlogPosts1.Count, firstArchiveItem?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPostHeads_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(BlogMetaTestData.BlogKey1, BlogMetaTestData.BlogKey2).ToList();

            // Assert
            var archiveContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey).ToList();
            var service = BlogServiceTestFactory.Create(mockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(BlogMetaTestData.BlogKey).ToList();

            // Assert
            var tagsContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var postHeads = service.ListPostHeads($"NonExisting_{BlogMetaTestData.BlogKey}");

            // Assert
            Assert.False(postHeads.Any());
        }

        [Fact]
        public void ListPostHeads_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var postHeads = service.ListPostHeads(BlogMetaTestData.BlogKey1).ToList();

            // Assert
            var archiveContainsBlogKeyMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPosts_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var posts = service.ListPosts(BlogMetaTestData.BlogKey1, BlogMetaTestData.BlogKey2).ToList();

            // Assert
            var archiveContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey).ToList();
            var service = BlogServiceTestFactory.Create(mockBlogPosts);

            // Act
            var posts = service.ListPosts(BlogMetaTestData.BlogKey).ToList();

            // Assert
            var tagsContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var posts = service.ListPosts($"NonExisting_{BlogMetaTestData.BlogKey}");

            // Assert
            Assert.False(posts.Any());
        }

        [Fact]
        public void ListPosts_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var posts = service.ListPosts(BlogMetaTestData.BlogKey1).ToList();

            // Assert
            var archiveContainsBlogKeyMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListTags_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var tags = service.ListTags(BlogMetaTestData.BlogKey1, BlogMetaTestData.BlogKey2).ToList();

            // Assert
            var tag5 = tags.FirstOrDefault(x => x.Name.EndsWith(" 5"));
            var tag11 = tags.FirstOrDefault(x => x.Name.EndsWith(" 11"));

            var archiveContainsAllMockPosts = GetTagsAllContainsBlogPostTags(tags, allMockBlogPosts);

            Assert.Equal(5, tag5?.Count);
            Assert.Equal(4, tag11?.Count);
            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListTags_ExistingBlogKey_ReturnsAll()
        {
            // Arrange
            var mockBlogPosts = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey).ToList();
            var service = BlogServiceTestFactory.Create(mockBlogPosts);

            // Act
            var tags = service.ListTags(BlogMetaTestData.BlogKey).ToList();

            // Assert
            var tagsContainsAllMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListTags_NonExistingBlogKeys_ReturnsNull()
        {
            // Arrange
            var service = BlogServiceTestFactory.Create();

            // Act
            var tags = service.ListTags($"NonExisting_{BlogMetaTestData.BlogKey}");

            // Assert
            Assert.False(tags.Any());
        }

        [Fact]
        public void ListTags_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            // Arrange
            var mockBlogPosts1 = BlogPostTestData.CreateCollection(0, 5, BlogMetaTestData.BlogKey1).ToList();
            var mockBlogPosts2 = BlogPostTestData.CreateCollection(5, 5, BlogMetaTestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = BlogServiceTestFactory.Create(allMockBlogPosts);

            // Act
            var tags = service.ListTags(BlogMetaTestData.BlogKey1).ToList();

            // Assert
            var tag5 = tags.FirstOrDefault(x => x.Name.EndsWith(" 5"));

            var archiveContainsBlogKeyMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts1);

            Assert.Equal(5, tag5?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        private static bool GetArchiveAllContainsBlogPosts(
            ICollection<BlogArchiveItem> archive,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !archive.Any())
                return false;

            var archiveAllContainsBlogPosts = blogPosts.All(
                p => p.PublishedAt.HasValue && archive.Any(
                         a => p.PublishedAt.Value.Year == a.Date.Year && p.PublishedAt.Value.Month == a.Date.Month));
            return archiveAllContainsBlogPosts;
        }

        private static bool GetPostHeadsAllContainsBlogs(
            ICollection<BlogPostHead> postHeads,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !postHeads.Any())
                return false;

            var postHeadsAllContainsBlogPosts = blogPosts.All(
                p => postHeads.Any(
                    h => h.BlogKey == p.BlogKey && h.BlavenId == p.BlavenId && h.Hash == p.Hash && h.Title == p.Title));
            return postHeadsAllContainsBlogPosts;
        }

        private static bool GetPostsAllContainsBlogPosts(
            ICollection<BlogPost> blogPosts1,
            IEnumerable<BlogPost> blogPosts2,
            bool expectAny = true)
        {
            if (expectAny && !blogPosts1.Any())
                return false;

            var postHeadsAllContainsBlogPosts = blogPosts2.All(
                p2 => blogPosts1.Any(
                    p1 => p1.BlogKey == p2.BlogKey && p1.BlavenId == p2.BlavenId && p1.Hash == p2.Hash
                          && p1.Title == p2.Title));
            return postHeadsAllContainsBlogPosts;
        }

        private static bool GetTagsAllContainsBlogPostTags(
            ICollection<BlogTagItem> tags,
            IEnumerable<BlogPost> blogPosts,
            bool expectAny = true)
        {
            if (expectAny && !tags.Any())
                return false;

            var tagsAllContainsBlogPosts = blogPosts.All(p => tags.Any(t => p.TagTexts.Contains(t.Name)));
            return tagsAllContainsBlogPosts;
        }
    }
}
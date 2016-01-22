using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Data.Tests;
using Xunit;

namespace Blaven.Tests
{
    public class BlogQueryServiceTest
    {
        [Fact]
        public void ctor_NullBlogSettings_ShouldNotThrow()
        {
            var repository = new MockRepository();
            var service = new BlogQueryService(repository, blogSettings: null);

            Assert.NotNull(service);
        }

        [Fact]
        public void ctor_NullRepository_ShouldThrow()
        {
            Assert.Throws<ArgumentNullException>(() => new BlogQueryService(repository: null));
        }

        [Fact]
        public void GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta()
        {
            var blogMetas = new[] { new BlogMeta { BlogKey = TestData.BlogKey, Name = TestData.BlogMetaName } };
            var service = GetBlogQueryService(blogMetas: blogMetas);

            var meta = service.GetBlogMeta(TestData.BlogKey);

            Assert.Equal(TestData.BlogKey, meta.BlogKey);
            Assert.Equal(TestData.BlogMetaName, meta.Name);
        }

        [Fact]
        public void GetBlogMeta_EmptyExistingBlogKey_ReturnsFirstBlogMeta()
        {
            var blogMetas = new[]
                                {
                                    new BlogMeta { BlogKey = TestData.BlogKey1, Name = TestData.BlogMetaName },
                                    new BlogMeta { BlogKey = TestData.BlogKey2, Name = $"{TestData.BlogMetaName}_Name" }
                                };
            var service = GetBlogQueryService(blogMetas: blogMetas);

            var meta = service.GetBlogMeta();

            Assert.Equal(TestData.BlogKeys.First(), meta.BlogKey);
            Assert.Equal(TestData.BlogMetaName, meta.Name);
        }

        [Fact]
        public void GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            var service = GetBlogQueryService();

            var meta = service.GetBlogMeta($"NonExisting_{TestData.BlogKey}");

            Assert.Null(meta);
        }

        [Fact]
        public void GetPost_ExistingBlogKey_ReturnsBlogPost()
        {
            var mockBlogPost = TestData.GetBlogPost(TestData.BlogKey);
            var service = GetBlogQueryService(blogPosts: new[] { mockBlogPost });

            var post = service.GetPost(mockBlogPost.BlavenId, TestData.BlogKey);

            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public void GetPost_NonExistingBlogKey_ReturnsNull()
        {
            var service = GetBlogQueryService();

            var post = service.GetPost($"NonExisting_{TestData.BlogKey}", string.Empty);

            Assert.Null(post);
        }

        [Fact]
        public void GetPostBySourceId_ExistingBlogKey_ReturnsBlogPost()
        {
            var mockBlogPost = TestData.GetBlogPost(TestData.BlogKey);
            var service = GetBlogQueryService(blogPosts: new[] { mockBlogPost });

            var post = service.GetPostBySourceId(mockBlogPost.SourceId, TestData.BlogKey);

            Assert.Equal(mockBlogPost.BlogKey, post.BlogKey);
            Assert.Equal(mockBlogPost.Content, post.Content);
        }

        [Fact]
        public void GetPostBySourceId_NonExistingBlogKey_ReturnsNull()
        {
            var service = GetBlogQueryService();

            var post = service.GetPostBySourceId($"NonExisting_{TestData.BlogKey}", string.Empty);

            Assert.Null(post);
        }

        [Fact]
        public void ListArchive_ExistingBlogKey_ReturnsAll()
        {
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogQueryService(mockBlogPosts);

            var archive = service.ListArchive(TestData.BlogKey).ToList();

            bool archiveContainsAllMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListArchive_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(35, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var archive = service.ListArchive(TestData.BlogKey2).ToList();

            var firstArchiveItem = archive.FirstOrDefault();

            bool archiveContainsBlogKeyMockPosts = GetArchiveAllContainsBlogPosts(archive, mockBlogPosts2);

            Assert.Equal(mockBlogPosts1.Count, firstArchiveItem?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListArchive_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(35, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var archive = service.ListArchive(TestData.BlogKey1, TestData.BlogKey2).ToList();

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
            var service = GetBlogQueryService();

            var archive = service.ListArchive($"NonExisting_{TestData.BlogKey}");

            Assert.False(archive.Any());
        }

        [Fact]
        public void ListTags_ExistingBlogKey_ReturnsAll()
        {
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogQueryService(mockBlogPosts);

            var tags = service.ListTags(TestData.BlogKey).ToList();

            bool tagsContainsAllMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListTags_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var tags = service.ListTags(TestData.BlogKey1).ToList();

            var tag5 = tags.FirstOrDefault(x => x.Name.EndsWith(" 5"));

            bool archiveContainsBlogKeyMockPosts = GetTagsAllContainsBlogPostTags(tags, mockBlogPosts1);

            Assert.Equal(5, tag5?.Count);
            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListTags_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var tags = service.ListTags(TestData.BlogKey1, TestData.BlogKey2).ToList();

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
            var service = GetBlogQueryService();

            var tags = service.ListTags($"NonExisting_{TestData.BlogKey}");

            Assert.False(tags.Any());
        }

        [Fact]
        public void ListPostHeads_ExistingBlogKey_ReturnsAll()
        {
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogQueryService(mockBlogPosts);

            var postHeads = service.ListPostHeads(TestData.BlogKey).ToList();

            bool tagsContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var postHeads = service.ListPostHeads(TestData.BlogKey1).ToList();

            bool archiveContainsBlogKeyMockPosts = GetPostHeadsAllContainsBlogs(postHeads, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPostHeads_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var postHeads = service.ListPostHeads(TestData.BlogKey1, TestData.BlogKey2).ToList();

            bool archiveContainsAllMockPosts = GetPostHeadsAllContainsBlogs(postHeads, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPostHeads_NonExistingBlogKeys_ReturnsNull()
        {
            var service = GetBlogQueryService();

            var postHeads = service.ListPostHeads($"NonExisting_{TestData.BlogKey}");

            Assert.False(postHeads.Any());
        }

        [Fact]
        public void ListPosts_ExistingBlogKey_ReturnsAll()
        {
            var mockBlogPosts = TestData.GetBlogPosts(0, 5, TestData.BlogKey).ToList();
            var service = GetBlogQueryService(mockBlogPosts);

            var posts = service.ListPosts(TestData.BlogKey).ToList();

            bool tagsContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts);

            Assert.True(tagsContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_SpecificBlogKey_ReturnsWithBlogPostsOnlyForBlogKey()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var posts = service.ListPosts(TestData.BlogKey1).ToList();

            bool archiveContainsBlogKeyMockPosts = GetPostsAllContainsBlogPosts(posts, mockBlogPosts1);

            Assert.True(archiveContainsBlogKeyMockPosts);
        }

        [Fact]
        public void ListPosts_AllBlogKeys_ReturnsWithAllBlogPosts()
        {
            var mockBlogPosts1 = TestData.GetBlogPosts(0, 5, TestData.BlogKey1).ToList();
            var mockBlogPosts2 = TestData.GetBlogPosts(5, 5, TestData.BlogKey2).ToList();
            var allMockBlogPosts = mockBlogPosts1.Concat(mockBlogPosts2).ToList();

            var service = GetBlogQueryService(allMockBlogPosts);

            var posts = service.ListPosts(TestData.BlogKey1, TestData.BlogKey2).ToList();

            bool archiveContainsAllMockPosts = GetPostsAllContainsBlogPosts(posts, allMockBlogPosts);

            Assert.True(archiveContainsAllMockPosts);
        }

        [Fact]
        public void ListPosts_NonExistingBlogKeys_ReturnsNull()
        {
            var service = GetBlogQueryService();

            var posts = service.ListPosts($"NonExisting_{TestData.BlogKey}");

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

        private static BlogQueryService GetBlogQueryService(
            IEnumerable<BlogPost> blogPosts = null,
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<string> blogSettingBlogKeys = null,
            int funcSleep = 100)
        {
            var blogSettings =
                (blogSettingBlogKeys ?? TestData.BlogKeys).Select(x => new BlogSetting(x, $"{x}_Id", $"{x}_Name"))
                    .ToList();

            var repository = MockRepository.Create(blogPosts, blogMetas, funcSleep);

            var service = new BlogQueryService(repository, blogSettings);
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
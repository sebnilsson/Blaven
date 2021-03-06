﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Blaven.DataStorage.Testing;
using Blaven.Testing;
using Xunit;

namespace Blaven.DataStorage.RavenDb.Tests
{
    public class RavenDbRepositoryTest : RepositoryTestBase
    {
        [Fact]
        public override void GetAllBlogMetas_ReturnsBlogMetasWithAllFieldValues()
        {
            // Arrange & Act & Assert
            base.GetAllBlogMetas_ReturnsBlogMetasWithAllFieldValues();
        }

        [Theory]
        [InlineData(BlogMetaTestData.BlogKey1)]
        [InlineData(BlogMetaTestData.BlogKey2)]
        [InlineData(BlogMetaTestData.BlogKey3)]
        public override async Task GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta(string blogKey)
        {
            // Arrange & Act & Assert
            await base.GetBlogMeta_ExistingBlogKey_ReturnsBlogMeta(blogKey);
        }

        [Fact]
        public override async Task GetBlogMeta_NonExistingBlogKey_ReturnsNull()
        {
            // Arrange & Act & Assert
            await base.GetBlogMeta_NonExistingBlogKey_ReturnsNull();
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPost_ExistingBlavenId_ReturnsPost(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPost_ExistingBlavenId_ReturnsPost(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPost_ExistingBlavenIdAndNonExistingBlogKey_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPost_ExistingBlavenIdAndNonExistingBlogKey_ReturnsNull(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPost_NonExistingBlavenId_ReturnsNull(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPost_NonExistingBlavenId_ReturnsNull(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPostBySourceId_ExistingBlogKeyAndNotExistingSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPostBySourceId_ExistingBlogKeyAndNotExistingSourceId_ReturnsNull(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPostBySourceId_ExistingBlogKeyAndSourceId_ReturnsBlogPost(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPostBySourceId_ExistingBlogKeyAndSourceId_ReturnsBlogPost(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPostBySourceId_ExistingBlogKeyAndUpperCaseSourceId_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPostBySourceId_ExistingBlogKeyAndUpperCaseSourceId_ReturnsNull(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override async Task GetPostBySourceId_ExistingSourceIdAndNonExistingBlogKey_ReturnsNull(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            await base.GetPostBySourceId_ExistingSourceIdAndNonExistingBlogKey_ReturnsNull(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListArchive_ExistingBlogKey_ReturnsArchive(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListArchive_ExistingBlogKey_ReturnsArchive(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListArchive_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostHeads_ExistingBlogKey_ReturnsPostHeads(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostHeads_ExistingBlogKey_ReturnsPostHeads(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostHeads_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostHeads_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPosts_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPosts_ExistingBlogKey_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPosts_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByArchive_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByArchive_ExistingBlogKey_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByArchive_ExistingBlogKeyAndNonExistingArchiveDate_ReturnsEmpty(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByArchive_ExistingBlogKeyAndNonExistingArchiveDate_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByArchive_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByArchive_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByTag_ExistingBlogKey_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByTag_ExistingBlogKey_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByTag_ExistingBlogKeyAndNonExistingTag_ReturnsEmpty(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByTag_ExistingBlogKeyAndNonExistingTag_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByTag_ExistingBlogKeyWithWrongCasing_ReturnsPosts(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByTag_ExistingBlogKeyWithWrongCasing_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListPostsByTag_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListPostsByTag_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Fact]
        public override void ListTags_DifferentCasingTags_ReturnsGroupedTags()
        {
            // Arrange & Act & Assert
            base.ListTags_DifferentCasingTags_ReturnsGroupedTags();
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListTags_ExistingBlogKey_ReturnsListTags(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListTags_ExistingBlogKey_ReturnsListTags(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void ListTags_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.ListTags_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void SearchPosts_ExistingBlogKeyAndContent_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.SearchPosts_ExistingBlogKeyAndContent_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void SearchPosts_ExistingBlogKeyAndTag_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.SearchPosts_ExistingBlogKeyAndTag_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void SearchPosts_ExistingBlogKeyAndTitle_ReturnsPosts(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.SearchPosts_ExistingBlogKeyAndTitle_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void SearchPosts_ExistingBlogKeyAndUpperCaseTitle_ReturnsPosts(
            IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.SearchPosts_ExistingBlogKeyAndUpperCaseTitle_ReturnsPosts(dbBlogPosts);
        }

        [Theory]
        [MemberData(
            nameof(BlogPostTheoryData.GetDbBlogPostsForSingleAndMultipleKeys),
            0,
            5,
            MemberType = typeof(BlogPostTheoryData))]
        public override void SearchPosts_NonExistingBlogKey_ReturnsEmpty(IEnumerable<BlogPost> dbBlogPosts)
        {
            // Arrange & Act & Assert
            base.SearchPosts_NonExistingBlogKey_ReturnsEmpty(dbBlogPosts);
        }

        protected override IRepository GetRepository(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null)
        {
            var documentStore = EmbeddableDocumentStoreTestFactory.CreateWithData(
                blogMetas: blogMetas,
                blogPosts: blogPosts);

            var repository = new RavenDbRepository(documentStore);
            return repository;
        }
    }
}
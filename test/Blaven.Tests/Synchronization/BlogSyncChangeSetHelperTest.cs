using System.Linq;

using Blaven.Tests;
using Xunit;

namespace Blaven.Synchronization.Tests
{
    public class BlogSyncChangeSetHelperTest
    {
        [Fact]
        public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoDeletedPosts()
        {
            // Arrange
            var sourcePosts = BlogPostTestData.CreateCollection(0, 0).ToList();
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dbPostList = dataStoragePosts.OfType<BlogPostBase>().ToList();
            var lastUpdatedAt = dataStoragePosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

            // Act
            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dbPostList);

            // Assert
            Assert.Equal(0, changeSet.DeletedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_NoLastUpdatedAt_AllPostsDeleted()
        {
            // Arrange
            var sourcePosts = BlogPostTestData.CreateCollection(0, 0).ToList();
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();

            // Act
            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

            // Assert
            Assert.Equal(5, changeSet.DeletedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoUpdatedBlogPosts()
        {
            // Arrange
            var sourcePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();
            var lastUpdatedAt = dataStoragePosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

            // Act
            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

            // Assert
            Assert.Equal(0, changeSet.UpdatedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_NoLastUpdatedAt_AllPostsUpdated()
        {
            // Arrange
            var sourcePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dataStoragePosts = BlogPostTestData.CreateCollection(0, 5).ToList();
            var dataStoragePostList = dataStoragePosts.OfType<BlogPostBase>().ToList();

            // Act
            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(BlogMetaTestData.BlogKey, sourcePosts, dataStoragePostList);

            // Assert
            Assert.Equal(5, changeSet.UpdatedBlogPosts.Count);
        }
    }
}
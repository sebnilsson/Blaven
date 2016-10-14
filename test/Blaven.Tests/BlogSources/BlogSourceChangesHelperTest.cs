using System.Linq;

using Blaven.Tests;
using Xunit;

namespace Blaven.BlogSources.Tests
{
    public class BlogSourceChangesHelperTest
    {
        [Fact]
        public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoDeletedPosts()
        {
            // Arrange
            var sourcePosts = TestData.GetBlogPosts(0, 0).ToList();
            var dbPosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPostList = dbPosts.OfType<BlogPostBase>().ToList();
            var lastUpdatedAt = dbPosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();
            
            // Act
            var changeSet = BlogSourceChangesHelper.GetChangeSet(
                TestData.BlogKey,
                sourcePosts,
                dbPostList,
                lastUpdatedAt);

            // Assert
            Assert.Equal(0, changeSet.DeletedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_NoLastUpdatedAt_AllPostsDeleted()
        {
            // Arrange
            var sourcePosts = TestData.GetBlogPosts(0, 0).ToList();
            var dbPosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPostList = dbPosts.OfType<BlogPostBase>().ToList();

            // Act
            var changeSet = BlogSourceChangesHelper.GetChangeSet(
                TestData.BlogKey,
                sourcePosts,
                dbPostList,
                lastUpdatedAt: null);

            // Assert
            Assert.Equal(5, changeSet.DeletedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_LastUpdatedAtAfterLastDbPost_NoUpdatedBlogPosts()
        {
            // Arrange
            var sourcePosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPostList = dbPosts.OfType<BlogPostBase>().ToList();
            var lastUpdatedAt = dbPosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

            // Act
            var changeSet = BlogSourceChangesHelper.GetChangeSet(
                TestData.BlogKey,
                sourcePosts,
                dbPostList,
                lastUpdatedAt);

            // Assert
            Assert.Equal(0, changeSet.UpdatedBlogPosts.Count);
        }

        [Fact]
        public void GetChangeSet_NoLastUpdatedAt_AllPostsUpdated()
        {
            // Arrange
            var sourcePosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPosts = TestData.GetBlogPosts(0, 5).ToList();
            var dbPostList = dbPosts.OfType<BlogPostBase>().ToList();

            // Act
            var changeSet = BlogSourceChangesHelper.GetChangeSet(
                TestData.BlogKey,
                sourcePosts,
                dbPostList,
                lastUpdatedAt: null);

            // Assert
            Assert.Equal(5, changeSet.UpdatedBlogPosts.Count);
        }
    }
}
using System.Linq;

using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class PagingUtilityTest
    {
        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(21)]
        public void GetPaged_PageIndexZero_ShouldReturnFirstPage(int pageSize)
        {
            // Arrange
            var firstPageBlogPosts = BlogPostTestData.CreateCollection(0, pageSize).ToList();
            var secondPageBlogPosts = BlogPostTestData.CreateCollection(pageSize, pageSize);
            var allBlogPosts = firstPageBlogPosts.Concat(secondPageBlogPosts);

            // Act
            var pagedBlogPosts = PagingUtility.GetPaged(allBlogPosts, pageSize, pageIndex: 0).ToList();

            // Assert
            bool pagedBlogPostsSequenceEquals = pagedBlogPosts.SequenceEqual(firstPageBlogPosts);

            Assert.True(pagedBlogPostsSequenceEquals);
            Assert.Equal(pageSize, pagedBlogPosts.Count);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(7)]
        [InlineData(21)]
        public void GetPaged_PageIndexOne_ShouldReturnSecondPage(int pageSize)
        {
            // Arrange
            var firstPageBlogPosts = BlogPostTestData.CreateCollection(0, pageSize);
            var secondPageBlogPosts = BlogPostTestData.CreateCollection(pageSize, pageSize).ToList();
            var allBlogPosts = firstPageBlogPosts.Concat(secondPageBlogPosts);

            // Act
            var pagedBlogPosts = PagingUtility.GetPaged(allBlogPosts, pageSize, pageIndex: 1).ToList();

            // Assert
            bool pagedBlogPostsSequenceEquals = pagedBlogPosts.SequenceEqual(secondPageBlogPosts);

            Assert.True(pagedBlogPostsSequenceEquals);
            Assert.Equal(pageSize, pagedBlogPosts.Count);
        }
    }
}
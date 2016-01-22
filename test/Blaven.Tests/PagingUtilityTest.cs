using System.Linq;

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
            var firstPageBlogPosts = TestData.GetBlogPosts(0, pageSize).ToList();
            var secondPageBlogPosts = TestData.GetBlogPosts(pageSize, pageSize);
            var allBlogPosts = firstPageBlogPosts.Concat(secondPageBlogPosts);

            var pagedBlogPosts = PagingUtility.GetPaged(allBlogPosts, pageSize, pageIndex: 0).ToList();

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
            var firstPageBlogPosts = TestData.GetBlogPosts(0, pageSize);
            var secondPageBlogPosts = TestData.GetBlogPosts(pageSize, pageSize).ToList();
            var allBlogPosts = firstPageBlogPosts.Concat(secondPageBlogPosts);

            var pagedBlogPosts = PagingUtility.GetPaged(allBlogPosts, pageSize, pageIndex: 1).ToList();

            bool pagedBlogPostsSequenceEquals = pagedBlogPosts.SequenceEqual(secondPageBlogPosts);

            Assert.True(pagedBlogPostsSequenceEquals);
            Assert.Equal(pageSize, pagedBlogPosts.Count);
        }
    }
}
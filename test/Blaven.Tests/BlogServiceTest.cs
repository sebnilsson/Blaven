using System.Threading.Tasks;
using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class BlogServiceTest
    {
        protected readonly ServicesContext ServicesContext;

        public BlogServiceTest()
        {
            ServicesContext = new ServicesContext();
        }

        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            ServicesContext.ConfigBlogSource(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            ServicesContext.ConfigStorageSyncRepo(
                BlogPostTestFactory.CreateList(2, 3));

            var blogService = ServicesContext.GetBlogService();
            var syncService = ServicesContext.GetSyncService();

            // Act
            await syncService.Synchronize();

            // Assert
            var posts = await blogService.ListPostHeaders();

            Assert.Equal(2, posts.Count);
        }
    }
}

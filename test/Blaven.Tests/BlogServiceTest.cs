using System.Threading.Tasks;
using Blaven.Testing;
using Xunit;

namespace Blaven.Tests
{
    public class BlogServiceTest
    {
        protected readonly TestContext Context;

        public BlogServiceTest()
        {
            Context = new TestContext();
        }

        [Fact]
        public async Task Synchronize_ContainsInserts_ReturnsInserts()
        {
            // Arrange
            Context.ConfigBlogSource(
                BlogPostTestFactory.CreateList(1, 2, 3, 4));

            Context.ConfigStorageSyncRepo(
                BlogPostTestFactory.CreateList(2, 3));

            var blogService = Context.GetBlogService();
            var syncService = Context.GetSyncService();

            // Act
            await syncService.Synchronize();

            // Assert
            var posts = await blogService.ListPostHeaders();

            Assert.Equal(4, posts.Count);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blaven.Data.EntityFramework.Tests
{
    public static class BlavenDbContextTestFactory
    {
        public static BlavenDbContext Create()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();


            var optionsBuilder = new DbContextOptionsBuilder<BlavenDbContext>();
            optionsBuilder.UseInMemoryDatabase().UseInternalServiceProvider(serviceProvider);

            var dbContext = new BlavenDbContext(optionsBuilder.Options);
            return dbContext;
        }
    }
}
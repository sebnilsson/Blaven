using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework.Tests
{
    public static class BlavenDbContextTestFactory
    {
        public static BlavenDbContext Create(string databaseName = null)
        {
            var options = GetDbContextOptions(databaseName);

            var dbContext = new BlavenDbContext(options);
            return dbContext;
        }

        public static BlavenDbContext CreateWithData(
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null,
            string databaseName = null)
        {
            var dbContext = Create(databaseName);

            if (blogMetas != null || blogPosts != null)
            {
                if (blogMetas != null)
                {
                    dbContext.BlogMetas.AddRange(blogMetas);
                }
                if (blogPosts != null)
                {
                    dbContext.BlogPosts.AddRange(blogPosts);
                }

                dbContext.SaveChanges();
            }

            return dbContext;
        }

        private static DbContextOptions<BlavenDbContext> GetDbContextOptions(string databaseName)
        {
            databaseName = databaseName ?? Convert.ToString(Guid.NewGuid());

            var optionsBuilder = new DbContextOptionsBuilder<BlavenDbContext>().UseInMemoryDatabase(databaseName);

            var options = optionsBuilder.Options;
            return options;
        }
    }
}
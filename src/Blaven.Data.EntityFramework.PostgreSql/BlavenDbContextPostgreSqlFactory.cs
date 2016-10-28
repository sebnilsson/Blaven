using System;

using Microsoft.EntityFrameworkCore;

namespace Blaven.Data.EntityFramework.PostgreSql
{
    public static class BlavenDbContextPostgreSqlFactory
    {
        public static BlavenDbContext Create(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException();
            }

            var optionsBuilder = new DbContextOptionsBuilder<BlavenDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            var dbContext = new BlavenDbContext(optionsBuilder.Options);
            //dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}
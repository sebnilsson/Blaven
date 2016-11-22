using System;

using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace Blaven.DataStorage.EntityFramework.PostgreSql
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

            var dbContext = new BlavenDbContext(optionsBuilder.Options, onModelCreating: OnModelCreating);
            //dbContext.Database.EnsureCreated();

            return dbContext;
        }

        private static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("hstore");
        }
    }
}
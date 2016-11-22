using System;

using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework
{
    public class BlavenDbContext : DbContext
    {
        private readonly Action<ModelBuilder> onModelCreating;

        public BlavenDbContext(DbContextOptions<BlavenDbContext> options, Action<ModelBuilder> onModelCreating = null)
            : base(options)
        {
            this.onModelCreating = onModelCreating;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.onModelCreating?.Invoke(modelBuilder);
        }

        public DbSet<BlogAuthor> BlogAuthors { get; set; }

        public DbSet<BlogMeta> BlogMetas { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<BlogPostTag> BlogPostTags { get; set; }
    }
}
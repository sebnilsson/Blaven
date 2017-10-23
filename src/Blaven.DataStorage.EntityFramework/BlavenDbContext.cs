using System;
using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework
{
    public class BlavenDbContext : DbContext
    {
        private readonly Action<ModelBuilder> _onModelCreating;

        public BlavenDbContext()
        {
        }

        public BlavenDbContext(Action<ModelBuilder> onModelCreating)
        {
            _onModelCreating = onModelCreating;
        }

        public BlavenDbContext(DbContextOptions<BlavenDbContext> options, Action<ModelBuilder> onModelCreating = null)
            : base(options)
        {
            Options = options;

            _onModelCreating = onModelCreating;
        }

        public DbSet<BlogAuthor> BlogAuthors { get; set; }

        public DbSet<BlogMeta> BlogMetas { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<BlogPostTag> BlogPostTags { get; set; }

        internal DbContextOptions<BlavenDbContext> Options { get; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _onModelCreating?.Invoke(modelBuilder);
        }
    }
}
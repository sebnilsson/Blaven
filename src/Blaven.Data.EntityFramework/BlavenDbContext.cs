using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Blaven.Data.EntityFramework
{
    public class BlavenDbContext : DbContext
    {
        public BlavenDbContext(DbContextOptions<BlavenDbContext> options)
            : base(options)
        {
        }

        // TODO: Create DbBlogMeta, DbBlogPost, DbBlogPostTag
        public DbSet<BlogMeta> BlogMetas { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }
    }
}
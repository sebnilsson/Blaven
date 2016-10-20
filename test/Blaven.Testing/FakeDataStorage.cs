using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Data;
using Blaven.Synchronization;

namespace Blaven.Tests
{
    public class FakeDataStorage : IDataStorage
    {
        private readonly ICollection<BlogPost> blogPosts;

        public FakeDataStorage(IEnumerable<BlogPost> blogPosts = null)
        {
            this.blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
        }

        public Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            var lastUpdatedAt = this.blogPosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();
            return Task.FromResult(lastUpdatedAt);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetPostBases(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var posts =
                this.blogPosts.Where(x => x.BlogKey == blogSetting.BlogKey && x.UpdatedAt > lastUpdatedAt)
                    .Cast<BlogPostBase>()
                    .ToReadOnlyList();
            return Task.FromResult(posts);
        }

        public Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            return Task.CompletedTask;
        }

        public Task SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet)
        {
            return Task.CompletedTask;
        }
    }
}
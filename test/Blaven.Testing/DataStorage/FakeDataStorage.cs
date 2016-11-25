using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Synchronization;

namespace Blaven.DataStorage.Tests
{
    public class FakeDataStorage : IDataStorage
    {
        private readonly ICollection<BlogPost> blogPosts;

        public FakeDataStorage(IEnumerable<BlogPost> blogPosts = null)
        {
            this.blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
        }

        public event EventHandler<string> OnGetLastUpdatedAtRun;

        public event EventHandler<string> OnGetBlogPostsRun;

        public event EventHandler<string> OnSaveBlogMetaRun;

        public event EventHandler<string> OnSaveChangesRun;

        public Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            var lastUpdatedAt = this.blogPosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

            this.OnGetLastUpdatedAtRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(lastUpdatedAt);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var posts =
                this.blogPosts.Where(
                        x => x.BlogKey == blogSetting.BlogKey && (lastUpdatedAt == null || x.UpdatedAt > lastUpdatedAt))
                    .Cast<BlogPostBase>()
                    .ToReadOnlyList();

            this.OnGetBlogPostsRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(posts);
        }

        public Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            this.OnSaveBlogMetaRun?.Invoke(this, blogSetting.BlogKey);

            return Task.CompletedTask;
        }

        public Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet)
        {
            this.OnSaveChangesRun?.Invoke(this, blogSetting.BlogKey);

            return Task.CompletedTask;
        }
    }
}
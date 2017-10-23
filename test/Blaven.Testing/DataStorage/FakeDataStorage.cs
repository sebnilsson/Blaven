using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Synchronization;

namespace Blaven.DataStorage.Testing
{
    public class FakeDataStorage : IDataStorage
    {
        private readonly ICollection<BlogPost> _blogPosts;

        public FakeDataStorage(IEnumerable<BlogPost> blogPosts = null)
        {
            _blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
        }

        public event EventHandler<string> OnGetBlogPostsRun;

        public event EventHandler<string> OnGetLastUpdatedAtRun;

        public event EventHandler<string> OnSaveBlogMetaRun;

        public event EventHandler<string> OnSaveChangesRun;

        public Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var posts = _blogPosts
                .Where(x => x.BlogKey == blogSetting.BlogKey && (lastUpdatedAt == null || x.UpdatedAt > lastUpdatedAt))
                .Cast<BlogPostBase>()
                .ToReadOnlyList();

            OnGetBlogPostsRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(posts);
        }

        public Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            var lastUpdatedAt = _blogPosts.Select(x => x.UpdatedAt).OrderByDescending(x => x).FirstOrDefault();

            OnGetLastUpdatedAtRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(lastUpdatedAt);
        }

        public Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            OnSaveBlogMetaRun?.Invoke(this, blogSetting.BlogKey);

            return Task.CompletedTask;
        }

        public Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet)
        {
            OnSaveChangesRun?.Invoke(this, blogSetting.BlogKey);

            return Task.CompletedTask;
        }
    }
}
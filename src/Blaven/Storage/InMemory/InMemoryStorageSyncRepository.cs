using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.Queries;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorageSyncRepository : IStorageSyncRepository
    {
        private readonly IInMemoryStorage _inMemoryStorage;

        public InMemoryStorageSyncRepository(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage
                ?? throw new ArgumentNullException(nameof(inMemoryStorage));
        }

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var meta =
                _inMemoryStorage
                    .Metas
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAt(lastUpdatedAt)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var posts =
                _inMemoryStorage
                    .Posts
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(lastUpdatedAt)
                    .OfType<BlogPostBase>()
                    .ToList()
                     as IReadOnlyList<BlogPostBase>;

            return Task.FromResult(posts);
        }

        public Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            IEnumerable<BlogPost> insertedPosts,
            IEnumerable<BlogPost> updatedPosts,
            IEnumerable<BlogPostBase> deletedPosts,
            DateTimeOffset? lastUpdatedAt)
        {
            if (insertedPosts is null)
                throw new ArgumentNullException(nameof(insertedPosts));
            if (updatedPosts is null)
                throw new ArgumentNullException(nameof(updatedPosts));
            if (deletedPosts is null)
                throw new ArgumentNullException(nameof(deletedPosts));

            if (lastUpdatedAt == null)
            {
                _inMemoryStorage.RemovePosts(blogKey);
            }

            _inMemoryStorage.CreateOrUpdateMeta(blogKey, meta);

            foreach (var post in insertedPosts)
            {
                _inMemoryStorage.CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in updatedPosts)
            {
                _inMemoryStorage.CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in deletedPosts)
            {
                _inMemoryStorage.RemovePosts(blogKey, post.Id);
            }

            return TaskCompleted.Task;
        }
    }
}

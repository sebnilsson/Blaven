using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorageQueryRepository : StorageQueryRepositoryBase
    {
        public InMemoryStorageQueryRepository(
            IInMemoryStorage inMemoryStorage,
            IOptionsMonitor<BlogQueryOptions> options)
            : base(
                  GetBlogMetas(inMemoryStorage),
                  GetBlogPosts(inMemoryStorage),
                  options)
        {
        }

        private static IQueryable<BlogMeta> GetBlogMetas(
            IInMemoryStorage inMemoryStorage)
        {
            if (inMemoryStorage is null)
                throw new ArgumentNullException(nameof(inMemoryStorage));

            return inMemoryStorage.Metas;
        }

        private static IQueryable<BlogPost> GetBlogPosts(
            IInMemoryStorage inMemoryStorage)
        {
            if (inMemoryStorage is null)
                throw new ArgumentNullException(nameof(inMemoryStorage));

            return inMemoryStorage.Posts;
        }
    }
}

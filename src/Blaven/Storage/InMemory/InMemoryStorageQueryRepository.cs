using Microsoft.Extensions.Options;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorageQueryRepository : StorageQueryRepositoryBase
    {
        public InMemoryStorageQueryRepository(
            IInMemoryStorage inMemoryStorage,
            IOptionsMonitor<BlogQueryOptions> options)
            : base(
                  inMemoryStorage,
                  options)
        {
        }
    }
}

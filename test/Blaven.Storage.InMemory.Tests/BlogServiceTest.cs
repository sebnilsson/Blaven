using Microsoft.Extensions.DependencyInjection;

namespace Blaven.Storage.InMemory.Tests.Synchronization
{
    public class BlogServiceTest : Blaven.Tests.BlogServiceTest
    {
        public BlogServiceTest()
        {
            Context.Config(services =>
            {
                var inMemoryStorage = new InMemoryStorageRepository();

                services.AddSingleton<IStorageSyncRepository>(
                    inMemoryStorage);

                services.AddSingleton<IStorageQueryRepository>(
                    inMemoryStorage);
            });
        }
    }
}

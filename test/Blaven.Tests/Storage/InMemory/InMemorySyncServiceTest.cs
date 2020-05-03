using System;
using Blaven.DependencyInjection;
using Blaven.Synchronization.Tests;
using Microsoft.Extensions.DependencyInjection;

namespace Blaven.Storage.InMemory.Tests
{
    public class InMemorySyncServiceTest : SyncServiceTest
    {
        protected override Action<IServiceCollection>? GetContextConfig()
        {
            return services =>
            {
                services.AddBlavenInMemoryStorage();
            };
        }
    }
}

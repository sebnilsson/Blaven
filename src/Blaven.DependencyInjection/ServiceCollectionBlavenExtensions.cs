using System;
using Blaven.Storage;
using Blaven.Storage.InMemory;
using Blaven.Synchronization;
using Blaven.Transformation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Blaven.DependencyInjection
{
    public static class ServiceCollectionBlavenExtensions
    {
        public static IServiceCollection AddBlaven(
            this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services
                .TryAddSingleton<
                    IBlogPostStorageTransformService,
                    BlogPostStorageTransformService>();

            services
                .TryAddSingleton<
                    IBlogPostQueryTransformService,
                    BlogPostQueryTransformService>();

            services
                .TryAddTransient<
                    IBlogSyncService,
                    BlogSyncService>();

            services
                .TryAddTransient<
                    IBlogQueryService,
                    BlogQueryService>();

            return services;
        }

        public static IServiceCollection AddBlavenInMemoryStorage(
            this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services
                .TryAddSingleton<
                    IInMemoryStorage,
                    InMemoryStorage>();

            services
                .TryAddTransient<
                    IStorageSyncRepository,
                    InMemoryStorageSyncRepository>();

            services
                .TryAddTransient<
                    IStorageQueryRepository,
                    InMemoryStorageQueryRepository>();

            return services;
        }
    }
}

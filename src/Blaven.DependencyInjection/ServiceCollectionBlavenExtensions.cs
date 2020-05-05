using System;
using Blaven.BlogSources;
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
                .TryAddTransient<IBlogSyncService>(x =>
                {
                    var blogSource = x.GetRequiredService<IBlogSource>();

                    var storage = x.GetRequiredService<IStorageSyncRepository>();

                    var transformService =
                        x.GetRequiredService<IBlogPostStorageTransformService>();

                    return new BlogSyncService(
                        blogSource,
                        storage,
                        transformService);
                });

            services
                .TryAddSingleton<IBlogPostStorageTransformService>(x =>
                {
                    var transforms = x.GetServices<IBlogPostStorageTransform>();
                    AssertService(transforms);

                    return new BlogPostStorageTransformService(transforms);
                });

            services
                .TryAddSingleton<IBlogPostQueryTransformService>(x =>
                {
                    var transforms = x.GetServices<IBlogPostQueryTransform>();
                    AssertService(transforms);

                    return new BlogPostQueryTransformService(transforms);
                });

            services
                .TryAddTransient<IBlogQueryService>(x =>
                {
                    var repository =
                        x.GetRequiredService<IStorageQueryRepository>();

                    var transformService =
                        x.GetRequiredService<IBlogPostQueryTransformService>();

                    return new BlogQueryService(repository, transformService);
                });

            return services;
        }

        public static IServiceCollection AddBlavenInMemoryStorage(
            this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<
                IInMemoryStorage,
                InMemoryStorage>();

            services.TryAddSingleton<
                IStorageSyncRepository,
                InMemoryStorageSyncRepository>();

            services.TryAddSingleton<
                IStorageQueryRepository,
                InMemoryStorageQueryRepository>();

            return services;
        }

        private static void AssertService<T>(T service)
        {
            if (service == null)
            {
                throw new InvalidOperationException(
                    $"No service for type '{typeof(T).FullName}' has been registered.");
            }
        }
    }
}

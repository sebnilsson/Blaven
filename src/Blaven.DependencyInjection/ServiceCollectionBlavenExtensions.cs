using System;
using Blaven.BlogSource;
using Blaven.Storage;
using Blaven.Synchronization;
using Blaven.Synchronization.Transformation;
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
                .TryAddTransient<ISyncService>(x =>
                {
                    var blogSource = x.GetRequiredService<IBlogSource>();

                    var storage = x.GetRequiredService<IStorageSyncRepository>();

                    var transformerService =
                        x.GetRequiredService<ITransformerService>();

                    return new SyncService(
                        blogSource,
                        storage,
                        transformerService);
                });

            services
                .TryAddSingleton<ITransformerService>(x =>
                {
                    var transformers = x.GetServices<IBlogPostTransformer>();
                    AssertService(transformers);

                    return new TransformerService(transformers);
                });

            services
                .TryAddTransient<IBlogService>(x =>
                {
                    var repository =
                        x.GetRequiredService<IStorageQueryRepository>();

                    return new BlogService(repository);
                });

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

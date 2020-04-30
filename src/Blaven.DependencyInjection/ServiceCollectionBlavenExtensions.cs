using System;
using Blaven.BlogSource;
using Blaven.Storage;
using Blaven.Synchronization;
using Blaven.Transformers;
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
                .TryAddTransient<ISynchronizationService>(x =>
                {
                    var blogSource = x.GetService<IBlogSource>();
                    AssertService(blogSource);

                    var storage = x.GetService<IStorage>();
                    AssertService(storage);

                    return new SynchronizationService(blogSource, storage);
                });

            services
                .TryAddSingleton<IBlogPostTransformerService>(x =>
                {
                    var transformers = x.GetServices<IBlogPostTransformer>();
                    AssertService(transformers);

                    return new BlogPostTransformerService(transformers);
                });

            services
                .TryAddTransient<IBlogService>(x =>
                {
                    var repository = x.GetService<IBlogServiceRepository>();
                    AssertService(repository);

                    return new BlogService(repository);
                });

            return services;
        }

        private static void AssertService<T>(T service)
        {
            if (service == null)
            {
                throw new InvalidOperationException(
                    $"Failed to resolve type '{typeof(T).GetType().FullName}'." +
                    $" Register type in the service collection.");
            }
        }
    }
}

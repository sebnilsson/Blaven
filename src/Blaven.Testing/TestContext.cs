using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSource;
using Blaven.DependencyInjection;
using Blaven.Storage;
using Blaven.Storage.InMemory;
using Blaven.Synchronization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Blaven.Testing
{
    public class TestContext
    {
        private readonly Lazy<IServiceProvider> _serviceProvider;

        public TestContext(
            Action<IServiceCollection>? config = null)
        {
            ServiceCollection = new ServiceCollection();

            config?.Invoke(ServiceCollection);

            ServiceCollection.AddBlaven();

            RegisterDefaultServices();

            _serviceProvider = new Lazy<IServiceProvider>(
                () => ServiceCollection.BuildServiceProvider());
        }

        public IServiceCollection ServiceCollection { get; }

        public IServiceProvider Services => _serviceProvider.Value;

        public void ConfigBlogSource(
            IEnumerable<BlogPost>? blogSourcePosts = null)
        {
            if (blogSourcePosts == null)
            {
                return;
            }

            var config = new BlogSourceTestConfig(blogSourcePosts);

            ServiceCollection.AddSingleton(config);
        }

        public void ConfigStorageSyncRepo(
            IEnumerable<BlogPost>? storagePosts = null)
        {
            if (storagePosts == null)
            {
                return;
            }

            var config = new StorageSyncRepositoryConfig(storagePosts);

            ServiceCollection.AddSingleton(config);
        }

        public IBlogService GetBlogService()
        {
            return Services.GetRequiredService<IBlogService>();
        }

        public T GetService<T>()
        {
            return Services.GetService<T>();
        }

        public T GetRequiredService<T>()
        {
            return Services.GetRequiredService<T>();
        }

        public ISyncService GetSyncService()
        {
            return Services.GetRequiredService<ISyncService>();
        }

        private void RegisterDefaultServices()
        {
            ServiceCollection.TryAddSingleton(x =>
            {
                var config = x.GetService<BlogSourceTestConfig>();

                var blogSource = new Mock<IBlogSource>();

                if (config != null)
                {
                    var blogSourcePosts =
                        config.BlogSourcePosts.ToList()
                        as IReadOnlyList<BlogPost>;

                    blogSource.Setup(x =>
                        x.GetPosts(It.IsAny<BlogKey>(), It.IsAny<DateTimeOffset?>()))
                    .Returns(Task.FromResult(blogSourcePosts));
                }

                return blogSource.Object;
            });

            ServiceCollection.TryAddSingleton(x =>
            {
                var config = x.GetService<StorageSyncRepositoryConfig>();

                var blogSource = new Mock<IStorageSyncRepository>();

                if (config != null)
                {
                    var storagePosts =
                        config.StoragePosts.OfType<BlogPostBase>().ToList()
                        as IReadOnlyList<BlogPostBase>;

                    blogSource.Setup(x =>
                        x.GetPosts(It.IsAny<BlogKey>(), It.IsAny<DateTimeOffset?>()))
                    .Returns(Task.FromResult(storagePosts));
                }

                return blogSource.Object;
            });

            ServiceCollection.AddSingleton<IInMemoryStorage>(x =>
            {
                var config = x.GetService<StorageSyncRepositoryConfig>();

                var storagePosts =
                    config?.StoragePosts ?? Enumerable.Empty<BlogPost>();

                return new InMemoryStorage(
                    metas: Enumerable.Empty<BlogMeta>(),
                    posts: storagePosts);
            });
        }
    }
}

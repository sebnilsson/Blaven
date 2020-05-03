using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSource;
using Blaven.DependencyInjection;
using Blaven.Storage;
using Blaven.Synchronization;
using Blaven.Synchronization.Transformation;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Blaven.Testing
{
    public class ServicesContext
    {
        private readonly Lazy<IServiceProvider> _serviceProvider;

        public ServicesContext()
        {
            ServiceCollection = new ServiceCollection();

            ServiceCollection.AddBlaven();

            var transformerService = new Mock<ITransformerService>();

            ServiceCollection.AddSingleton(transformerService.Object);

            _serviceProvider = new Lazy<IServiceProvider>(
                () => ServiceCollection.BuildServiceProvider());
        }

        public IServiceCollection ServiceCollection { get; }

        public IServiceProvider Services => _serviceProvider.Value;

        public void Config(Action<IServiceCollection> config)
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            config.Invoke(ServiceCollection);
        }

        public void ConfigBlogSource(
            IEnumerable<BlogPost>? blogSourcePosts = null)
        {
            var blogSource = new Mock<IBlogSource>();

            if (blogSourcePosts != null)
            {
                blogSource.Setup(x =>
                    x.GetPosts(It.IsAny<BlogKey>(), It.IsAny<DateTimeOffset?>()))
                .Returns(
                    Task.FromResult(blogSourcePosts.ToList() as IReadOnlyList<BlogPost>));
            }

            ServiceCollection.AddSingleton(blogSource.Object);
        }

        public void ConfigStorageSyncRepo(
            IEnumerable<BlogPost>? storagePosts = null)
        {
            var storageSyncRepo = new Mock<IStorageSyncRepository>();

            if (storagePosts != null)
            {
                storageSyncRepo.Setup(x =>
                    x.GetPosts(It.IsAny<BlogKey>(), It.IsAny<DateTimeOffset?>()))
                .Returns(
                    Task.FromResult(storagePosts.ToList() as IReadOnlyList<BlogPostBase>));
            }

            ServiceCollection.AddSingleton(storageSyncRepo.Object);
        }

        public IBlogService GetBlogService()
        {
            return Services.GetRequiredService<IBlogService>();
        }

        public ISyncService GetSyncService()
        {
            return Services.GetRequiredService<ISyncService>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.DataSources;
using Blaven.RavenDb;

namespace Blaven
{
    internal class BlogRefreshService
    {
        private readonly BlogServiceConfig config;

        private readonly Repository repository;

        public BlogRefreshService(Repository repository, BlogServiceConfig config)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.repository = repository;
            this.config = config;
        }

        public IEnumerable<BlogRefreshResult> Refresh(
            IEnumerable<BlavenBlogSetting> settings, bool forceRefresh = false)
        {
            var synchronizer = new BlogRefreshServiceSynchronizer(
                setting => this.Refresh(setting), this.repository, this.config, forceRefresh);

            var results = synchronizer.RefreshSynchronized(settings);
            return results;
        }

        private async Task Refresh(BlavenBlogSetting setting, bool forceRefresh = false)
        {
            var dataSourceRefreshService = new DataSourceRefreshService(this.repository);
            var dataSourceResult = await TaskEx.Run(() => dataSourceRefreshService.Refresh(setting, forceRefresh));

            var repositoryRefreshService = new RepositoryRefreshService(this.repository, setting.BlogKey, forceRefresh);
            await repositoryRefreshService.Refresh(dataSourceResult);
        }
    }
}
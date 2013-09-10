using System;
using System.Collections.Generic;

using Blaven.RavenDb;

namespace Blaven.DataSources
{
    internal class DataSourceRefreshService
    {
        private readonly BlogServiceConfig config;

        private readonly Repository repository;

        public DataSourceRefreshService(BlogServiceConfig config, Repository repository)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.config = config;
            this.repository = repository;
        }

        public IEnumerable<RefreshSynchronizerResult> Refresh(
            IEnumerable<BlavenBlogSetting> settings, bool forceRefresh)
        {
            var synchronizerService = new RefreshSynchronizerService(
                this.RefreshDataSourceAndRepository, this.repository, this.config, forceRefresh);

            var results = synchronizerService.RefreshSynchronized(settings);
            return results;
        }

        private void RefreshDataSourceAndRepository(BlavenBlogSetting setting, bool forceRefresh)
        {
            var result = this.RefreshDataSource(setting, forceRefresh);

            this.repository.Refresh(setting.BlogKey, result, throwOnException: forceRefresh);
        }

        internal DataSourceRefreshResult RefreshDataSource(BlavenBlogSetting setting, bool forceRefresh)
        {
            string blogKey = setting.BlogKey;
            var lastRefresh = !forceRefresh ? this.repository.GetBlogRefreshTimestamp(blogKey) : null;

            var blogPostsMeta = this.repository.GetAllBlogPostMeta(blogKey);

            var blogInfo = this.repository.GetBlogInfo(blogKey);

            var refreshContext = new DataSourceRefreshContext
                                     {
                                         BlogInfoChecksum =
                                             (blogInfo != null) ? blogInfo.Checksum : null,
                                         BlogSetting = setting,
                                         ExistingBlogPostsMetas = blogPostsMeta,
                                         ForceRefresh = forceRefresh,
                                         LastRefresh = lastRefresh
                                     };

            var dataSource = setting.BlogDataSource;

            return dataSource.Refresh(refreshContext);
        }
    }
}
using System;

using Blaven.RavenDb;

namespace Blaven.DataSources
{
    internal class DataSourceRefreshService
    {
        private readonly Repository repository;

        public DataSourceRefreshService(Repository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            this.repository = repository;
        }

        internal DataSourceRefreshResult Refresh(BlavenBlogSetting setting, bool forceRefresh)
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
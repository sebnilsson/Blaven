using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.RavenDb;

namespace Blaven.DataSources
{
    internal class DataSourceRefreshService
    {
        private const int RefreshTimeoutSeconds = 30;

        private static readonly Dictionary<string, bool> BlogKeyIsRefreshing = new Dictionary<string, bool>();

        private readonly BlogServiceConfig config;

        private readonly RavenRepository repository;
        
        public DataSourceRefreshService(BlogServiceConfig config, RavenRepository repository)
        {
            this.config = config;
            this.repository = repository;
        }

        public IEnumerable<RefreshResult> Refresh(IEnumerable<BlavenBlogSetting> settings, bool forceRefresh)
        {
            var results = new List<RefreshResult>();
            Parallel.ForEach(
                settings,
                setting =>
                    {
                        var result = Refresh(setting, forceRefresh);
                        results.Add(result);
                    });

            var criticalErrors =
                results.Where(
                    x => x.ResultType == RefreshResultType.UpdateFailed && (!x.HasBlogAnyData || forceRefresh)).ToList();
            if (criticalErrors.Any())
            {
                throw new DataSourceRefresherException(criticalErrors);
            }

            return results.AsEnumerable();
        }

        public RefreshResult Refresh(BlavenBlogSetting setting, bool forceRefresh)
        {
            RefreshResult result = null;
            var measuredTime = StopwatchHelper.PerformMeasuredAction(
                () =>
                    { result = GetLockedResult(setting, forceRefresh); });

            result.ElapsedTime = measuredTime;

            if (!result.HasBlogAnyData)
            {
                repository.WaitForData(setting.BlogKey);
            }
            return result;
        }

        private RefreshResult GetLockedResult(BlavenBlogSetting setting, bool forceRefresh)
        {
            string lockKey = string.Format("DataSourceRefresher.GetRefreshResult_{0}", setting.BlogKey);

            var result = KeyLockService.PerformLockedFunction(lockKey, () => PerformRefresh(setting, forceRefresh));
            return result;
        }

        private RefreshResult PerformRefresh(BlavenBlogSetting setting, bool forceRefresh)
        {
            string blogKey = setting.BlogKey;

            var cancelRefresh = this.GetCancelRefresh(blogKey, forceRefresh);
            if (cancelRefresh != null)
            {
                return cancelRefresh;
            }

            BlogKeyIsRefreshing[blogKey] = true;

            try
            {
                var updateRepositoryTask = Task.Factory.StartNew(() => RefreshRepository(setting, forceRefresh));

                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                bool refreshSync = !this.config.RefreshAsync;
                if (forceRefresh || refreshSync || !hasBlogAnyData)
                {
                    updateRepositoryTask.Wait(TimeSpan.FromSeconds(RefreshTimeoutSeconds));

                    hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                    return new RefreshResult(blogKey, RefreshResultType.UpdateSync, hasBlogAnyData);
                }

                return new RefreshResult(blogKey, RefreshResultType.UpdateAsync);
            }
            catch (Exception ex)
            {
                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                return new RefreshResult(blogKey, RefreshResultType.UpdateFailed, hasBlogAnyData, ex);
            }
            finally
            {
                BlogKeyIsRefreshing[blogKey] = false;
            }
        }

        private RefreshResult GetCancelRefresh(string blogKey, bool forceRefresh)
        {
            if (GetIsBlogRefreshing(blogKey, forceRefresh))
            {
                bool hasBlogAnyData = this.repository.GetHasBlogAnyData(blogKey);
                return new RefreshResult(blogKey, RefreshResultType.CancelledIsRefreshing, hasBlogAnyData);
            }

            if (!forceRefresh && this.repository.GetIsBlogRefreshed(blogKey, this.config.CacheTime))
            {
                return new RefreshResult(blogKey, RefreshResultType.CancelledIsRefreshed);
            }

            return null;
        }

        private void RefreshRepository(BlavenBlogSetting setting, bool forceRefresh)
        {
            string blogKey = setting.BlogKey;
            var lastRefresh = !forceRefresh ? this.repository.GetBlogRefreshTimestamp(blogKey) : null;

            var blogPostsMeta = this.repository.GetAllBlogPostMeta(setting.BlogKey);

            var refreshContext = new DataSourceRefreshContext
                                     {
                                         BlogPostsMetas = blogPostsMeta,
                                         BlogSetting = setting,
                                         ForceRefresh = forceRefresh,
                                         LastRefresh = lastRefresh
                                     };
            var dataSource = setting.BlogDataSource;
            var refreshResult = dataSource.Refresh(refreshContext);
            
            repository.Refresh(blogKey, refreshResult, throwOnCritical: forceRefresh);
        }

        private static bool GetIsBlogRefreshing(string blogKey, bool forceRefresh)
        {
            return !forceRefresh && BlogKeyIsRefreshing.ContainsKey(blogKey) && BlogKeyIsRefreshing[blogKey];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.RavenDb;

namespace Blaven.DataSources
{
    internal class RefreshSynchronizerService
    {
        private const int RefreshTimeoutSeconds = 30;

        private static readonly Dictionary<string, bool> BlogKeyIsRefreshing = new Dictionary<string, bool>();

        private readonly BlogServiceConfig config;

        private readonly bool forceRefresh;

        private readonly Action<BlavenBlogSetting, bool> refreshAction;

        private readonly Repository repository;

        public RefreshSynchronizerService(
            Action<BlavenBlogSetting, bool> refreshAction,
            Repository repository,
            BlogServiceConfig config,
            bool forceRefresh)
        {
            this.refreshAction = refreshAction;
            this.repository = repository;
            this.config = config;
            this.forceRefresh = forceRefresh;
        }

        public IEnumerable<RefreshSynchronizerResult> RefreshSynchronized(IEnumerable<BlavenBlogSetting> settings)
        {
            var results = settings.AsParallel().Select(this.RefreshBlog).ToList();

            var criticalErrors =
                results.Where(
                    x =>
                    x.ResultType == RefreshSynchronizerResultType.UpdateFailed && (!x.HasBlogAnyData || this.forceRefresh))
                       .ToList();
            if (criticalErrors.Any())
            {
                throw new RefreshSynchronizerServiceException(criticalErrors);
            }

            return results;
        }

        private RefreshSynchronizerResult RefreshBlog(BlavenBlogSetting setting)
        {
            RefreshSynchronizerResult result = null;
            var measuredTime = StopwatchHelper.PerformMeasuredAction(() => { result = PerformRefreshLocked(setting); });

            result.ElapsedTime = measuredTime;

            if (result.ResultType != RefreshSynchronizerResultType.UpdateFailed && !result.HasBlogAnyData)
            {
                this.repository.WaitForData(setting.BlogKey);
            }

            return result;
        }

        private RefreshSynchronizerResult PerformRefreshLocked(BlavenBlogSetting setting)
        {
            string lockKey = string.Format("DataSourceRefreshService.GetLockedResult_{0}", setting.BlogKey);

            var result = KeyLockService.PerformLockedFunction(lockKey, () => PerformRefresh(setting));
            return result;
        }

        private RefreshSynchronizerResult PerformRefresh(BlavenBlogSetting setting)
        {
            string blogKey = setting.BlogKey;

            var cancelRefresh = this.GetCancelRefresh(blogKey);
            if (cancelRefresh != null)
            {
                return cancelRefresh;
            }

            BlogKeyIsRefreshing[blogKey] = true;

            try
            {
                var refreshTask = Task.Factory.StartNew(() => this.refreshAction(setting, forceRefresh));

                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                if (this.forceRefresh || this.config.RefreshAsync || !hasBlogAnyData)
                {
                    refreshTask.Wait(TimeSpan.FromSeconds(RefreshTimeoutSeconds));

                    hasBlogAnyData = this.repository.GetHasBlogAnyData(blogKey);
                    return new RefreshSynchronizerResult(
                        blogKey, RefreshSynchronizerResultType.UpdateSync, hasBlogAnyData);
                }

                return new RefreshSynchronizerResult(blogKey, RefreshSynchronizerResultType.UpdateAsync);
            }
            catch (Exception ex)
            {
                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                return new RefreshSynchronizerResult(
                    blogKey, RefreshSynchronizerResultType.UpdateFailed, hasBlogAnyData, ex);
            }
            finally
            {
                BlogKeyIsRefreshing[blogKey] = false;
            }
        }

        private RefreshSynchronizerResult GetCancelRefresh(string blogKey)
        {
            if (this.GetIsBlogRefreshing(blogKey))
            {
                bool hasBlogAnyData = this.repository.GetHasBlogAnyData(blogKey);
                return new RefreshSynchronizerResult(
                    blogKey, RefreshSynchronizerResultType.CancelledIsRefreshing, hasBlogAnyData);
            }

            if (!this.forceRefresh && this.repository.GetIsBlogRefreshed(blogKey, this.config.CacheTime))
            {
                return new RefreshSynchronizerResult(blogKey, RefreshSynchronizerResultType.CancelledIsRefreshed);
            }

            return null;
        }

        private bool GetIsBlogRefreshing(string blogKey)
        {
            return !this.forceRefresh && BlogKeyIsRefreshing.ContainsKey(blogKey) && BlogKeyIsRefreshing[blogKey];
        }
    }
}
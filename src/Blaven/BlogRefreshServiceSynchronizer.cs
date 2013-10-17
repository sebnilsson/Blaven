using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.RavenDb;

namespace Blaven
{
    internal class BlogRefreshServiceSynchronizer
    {
        private const int RefreshTimeoutSeconds = 30;

        private static readonly Dictionary<string, bool> BlogKeyIsRefreshing = new Dictionary<string, bool>();

        private readonly BlogServiceConfig config;

        private readonly bool forceRefresh;

        private readonly Repository repository;

        private readonly Action<BlavenBlogSetting> refreshAction;

        public BlogRefreshServiceSynchronizer(
            Action<BlavenBlogSetting> refreshAction, Repository repository, BlogServiceConfig config, bool forceRefresh)
        {
            if (refreshAction == null)
            {
                throw new ArgumentNullException("refreshAction");
            }
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this.refreshAction = refreshAction;
            this.repository = repository;
            this.config = config;
            this.forceRefresh = forceRefresh;
        }

        public IEnumerable<BlogRefreshResult> RefreshSynchronized(IEnumerable<BlavenBlogSetting> settings)
        {
            var results = settings.AsParallel().Select(this.RefreshBlog).ToList();

            var criticalErrors =
                results.Where(
                    x => x.ResultType == BlogRefreshResultType.UpdateFailed && (!x.HasBlogAnyData || this.forceRefresh))
                       .ToList();
            if (criticalErrors.Any())
            {
                throw new BlogRefreshServiceException(criticalErrors);
            }

            return results;
        }

        private BlogRefreshResult RefreshBlog(BlavenBlogSetting setting)
        {
            BlogRefreshResult result = null;
            var measuredTime = StopwatchHelper.PerformMeasuredAction(() => { result = RefreshBlogLocked(setting); });

            result.ElapsedTime = measuredTime;

            if (result.ResultType != BlogRefreshResultType.UpdateFailed && !result.HasBlogAnyData)
            {
                this.repository.WaitForData(setting.BlogKey);
            }

            return result;
        }

        private BlogRefreshResult RefreshBlogLocked(BlavenBlogSetting setting)
        {
            string lockKey = string.Format("RefreshMutexService.PerformRefreshLocked_{0}", setting.BlogKey);

            var result = KeyLockService.PerformLockedFunction(lockKey, () => PerformRefresh(setting));
            return result;
        }

        private BlogRefreshResult PerformRefresh(BlavenBlogSetting setting)
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
                var refreshTask = Task.Factory.StartNew(() => this.refreshAction(setting));

                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                if (this.forceRefresh || this.config.RefreshAsync || !hasBlogAnyData)
                {
                    refreshTask.Wait(TimeSpan.FromSeconds(RefreshTimeoutSeconds));

                    hasBlogAnyData = this.repository.GetHasBlogAnyData(blogKey);
                    return new BlogRefreshResult(blogKey, BlogRefreshResultType.UpdateSync, hasBlogAnyData);
                }

                return new BlogRefreshResult(blogKey, BlogRefreshResultType.UpdateAsync);
            }
            catch (Exception ex)
            {
                bool hasBlogAnyData = repository.GetHasBlogAnyData(blogKey);
                return new BlogRefreshResult(blogKey, BlogRefreshResultType.UpdateFailed, hasBlogAnyData, ex);
            }
            finally
            {
                BlogKeyIsRefreshing[blogKey] = false;
            }
        }

        private BlogRefreshResult GetCancelRefresh(string blogKey)
        {
            if (this.GetIsBlogRefreshing(blogKey))
            {
                bool hasBlogAnyData = this.repository.GetHasBlogAnyData(blogKey);
                return new BlogRefreshResult(blogKey, BlogRefreshResultType.CancelledIsRefreshing, hasBlogAnyData);
            }

            if (!this.forceRefresh && this.repository.GetIsBlogRefreshed(blogKey, this.config.CacheTime))
            {
                return new BlogRefreshResult(blogKey, BlogRefreshResultType.CancelledIsRefreshed);
            }

            return null;
        }

        private bool GetIsBlogRefreshing(string blogKey)
        {
            return !this.forceRefresh && BlogKeyIsRefreshing.ContainsKey(blogKey) && BlogKeyIsRefreshing[blogKey];
        }
    }
}
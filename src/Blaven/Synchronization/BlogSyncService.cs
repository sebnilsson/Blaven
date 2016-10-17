using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;

namespace Blaven.Synchronization
{
    public class BlogSyncService
    {
        private static readonly KeyLocker UpdateBlogLocker = new KeyLocker();

        private readonly BlogSettingsHelper blogSettings;

        public BlogSyncService(IBlogSource blogSource, IDataStorage dataStorage, params BlogSetting[] blogSettings)
            : this(GetConfig(blogSource, dataStorage, blogSettings))
        {
        }

        public BlogSyncService(BlogSyncConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.Config = config;

            this.blogSettings = new BlogSettingsHelper(config.BlogSettings);
        }

        public BlogSyncConfiguration Config { get; }

        public async Task<bool> IsUpdated(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var now = DateTime.Now;

            bool isUpdated = await this.IsUpdatedInternal(now, ensuredBlogKey);
            return isUpdated;
        }

        public async Task<IReadOnlyList<BlogSyncResult>> Update(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys).ToArray();

            var now = DateTime.Now;

            var result = await this.UpdateInternal(now, ensuredBlogKeys);
            return result;
        }

        public async Task<IReadOnlyList<BlogSyncResult>> UpdateAll(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys).ToArray();

            var now = DateTime.Now;

            var result = await this.UpdateAllInternal(now, ensuredBlogKeys);
            return result;
        }

        internal async Task<bool> IsUpdatedInternal(DateTime now, string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            bool isUpdated = await this.Config.DataCacheHandler.IsUpdated(now, blogKey);

            return isUpdated;
        }

        internal async Task<IReadOnlyList<BlogSyncResult>> UpdateAllInternal(DateTime now, params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var resultTasks =
                blogKeys.Select(async blogKey => await this.UpdateBlog(blogKey, now, forceShouldUpdate: true)).ToList();

            await Task.WhenAll(resultTasks);

            return resultTasks.Select(x => x.Result).ToReadOnlyList();
        }

        internal async Task<IReadOnlyList<BlogSyncResult>> UpdateInternal(DateTime now, params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var resultTasks = blogKeys.Select(async blogKey => await this.UpdateBlog(blogKey, now)).ToList();

            await Task.WhenAll(resultTasks);

            return resultTasks.Select(x => x.Result).ToReadOnlyList();
        }

        private BlogSetting GetBlogSetting(string blogKey)
        {
            var blogSetting = this.Config.TryGetBlogSetting(blogKey);
            if (blogSetting == null)
            {
                string message =
                    $"{this.Config.GetType().Name} did not contain any {nameof(BlogSetting)} with {nameof(blogKey)} '{blogKey}'.";
                throw new ArgumentOutOfRangeException(nameof(blogKey), message);
            }

            return blogSetting;
        }

        private async Task<BlogSyncResult> UpdateBlog(string blogKey, DateTime now, bool forceShouldUpdate = false)
        {
            var lockKey = blogKey.ToLowerInvariant();

            return
                await
                    UpdateBlogLocker.RunWithLock(
                        lockKey,
                        async () => await this.UpdateBlogInternal(blogKey, now, forceShouldUpdate));
        }

        private async Task<BlogSyncResult> UpdateBlogInternal(string blogKey, DateTime now, bool forceShouldUpdate)
        {
            var result = new BlogSyncResult(blogKey);

            try
            {
                var blogSetting = this.GetBlogSetting(blogKey);

                var lastUpdatedAt = await this.Config.DataStorage.GetLastUpdatedAt(blogSetting);

                bool isUpdated = !forceShouldUpdate && !(await this.IsUpdatedInternal(now, blogKey));
                if (isUpdated)
                {
                    return result;
                }

                await this.UpdateBlogData(blogSetting, lastUpdatedAt, result);

                await this.Config.DataCacheHandler.OnUpdated(now, blogKey);

                return result;
            }
            finally
            {
                result.OnDone();
            }
        }

        private async Task UpdateBlogData(BlogSetting blogSetting, DateTime? lastUpdatedAt, BlogSyncResult result)
        {
            var updateMetaTask = BlogSyncServiceMetaHelper.Update(blogSetting, lastUpdatedAt, this.Config);
            var updatePostsTask = BlogSyncServicePostsHelper.Update(blogSetting, lastUpdatedAt, this.Config);

            await Task.WhenAll(updateMetaTask, updatePostsTask);

            var meta = updateMetaTask.Result;
            var changeSet = updatePostsTask.Result;

            result.OnDataUpdated(meta, changeSet);

            var saveBlogMetaTask = this.Config.DataStorage.SaveBlogMeta(blogSetting, meta);
            var saveChangesTask = this.Config.DataStorage.SaveChanges(blogSetting, changeSet);

            await Task.WhenAll(saveBlogMetaTask, saveChangesTask);
        }

        private static BlogSyncConfiguration GetConfig(
            IBlogSource blogSource,
            IDataStorage dataStorage,
            IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSource == null)
            {
                throw new ArgumentNullException(nameof(blogSource));
            }
            if (dataStorage == null)
            {
                throw new ArgumentNullException(nameof(dataStorage));
            }

            var config = new BlogSyncConfiguration(
                             blogSource,
                             dataStorage,
                             dataCacheHandler: null,
                             slugProvider: null,
                             blavenIdProvider: null,
                             transformersProvider: null,
                             blogSettings: blogSettings ?? Enumerable.Empty<BlogSetting>());
            return config;
        }
    }
}
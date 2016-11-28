using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.DataStorage;

namespace Blaven.Synchronization
{
    public class BlogSyncService
    {
        private static readonly KeyLocker UpdateBlogLocker = new KeyLocker();

        private readonly BlogSettingsHelper blogSettings;

        private readonly BlogSyncServiceMetaHelper metaHelper;

        private readonly BlogSyncServicePostsHelper postsHelper;

        public BlogSyncService(IBlogSource blogSource, IDataStorage dataStorage, IEnumerable<BlogSetting> blogSettings)
            : this(BlogSyncConfiguration.Create(blogSource, dataStorage, blogSettings))
        {
        }

        public BlogSyncService(IBlogSource blogSource, IDataStorage dataStorage, params BlogSetting[] blogSettings)
            : this(blogSource, dataStorage, blogSettings?.AsEnumerable())
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

            this.metaHelper = new BlogSyncServiceMetaHelper(config);
            this.postsHelper = new BlogSyncServicePostsHelper(config);
        }

        public BlogSyncConfiguration Config { get; }

        public async Task<IReadOnlyList<BlogSyncResult>> Update(params BlogKey[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var result = await this.UpdateBlogs(ensuredBlogKeys, forceShouldUpdate: false);
            return result;
        }

        public async Task<IReadOnlyList<BlogSyncResult>> UpdateAll(params BlogKey[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var result = await this.UpdateBlogs(ensuredBlogKeys, forceShouldUpdate: true);
            return result;
        }

        private async Task<IReadOnlyList<BlogSyncResult>> UpdateBlogs(
            IEnumerable<string> blogKeys,
            bool forceShouldUpdate)
        {
            var resultTasks =
                blogKeys.Select(async blogKey => await this.UpdateBlog(blogKey, forceShouldUpdate)).ToList();

            await Task.WhenAll(resultTasks);

            return resultTasks.Select(x => x.Result).ToReadOnlyList();
        }

        private async Task<BlogSyncResult> UpdateBlog(string blogKey, bool forceShouldUpdate)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = new BlogSyncResult(blogKey);

            string lockKey = blogKey.ToLowerInvariant();

            try
            {
                var blogSetting = this.blogSettings.GetBlogSetting(blogKey);

                await UpdateBlogLocker.RunWithLock(
                    lockKey,
                    async () =>
                        {
                            var lastUpdatedAt = !forceShouldUpdate
                                                    ? await this.Config.DataStorage.GetLastUpdatedAt(blogSetting)
                                                    : null;

                            await this.UpdateBlogData(blogSetting, lastUpdatedAt, result);
                        });
            }
            finally
            {
                if (stopwatch != null)
                {
                    stopwatch.Stop();

                    result.Elapsed = stopwatch.Elapsed;
                }
            }

            return result;
        }

        private async Task UpdateBlogData(BlogSetting blogSetting, DateTime? lastUpdatedAt, BlogSyncResult result)
        {
            var updateMetaTask = this.metaHelper.Update(blogSetting, lastUpdatedAt);
            var updatePostsTask = this.postsHelper.Update(blogSetting, lastUpdatedAt);

            await Task.WhenAll(updateMetaTask, updatePostsTask);

            var meta = updateMetaTask.Result;
            var changeSet = updatePostsTask.Result;

            result.OnDataUpdated(meta, changeSet);

            var saveBlogMetaTask = (meta != null)
                                       ? this.Config.DataStorage.SaveBlogMeta(blogSetting, meta)
                                       : TaskHelper.CompletedTask;
            var saveChangesTask = (changeSet != null)
                                      ? this.Config.DataStorage.SaveChanges(blogSetting, changeSet)
                                      : TaskHelper.CompletedTask;

            await Task.WhenAll(saveBlogMetaTask, saveChangesTask);
        }
    }
}
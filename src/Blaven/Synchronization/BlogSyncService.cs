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
        private readonly BlogSettingsHelper _blogSettings;
        private readonly BlogSyncServiceMetaHelper _metaHelper;
        private readonly BlogSyncServicePostsHelper _postsHelper;

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
            Config = config ?? throw new ArgumentNullException(nameof(config));

            _blogSettings = new BlogSettingsHelper(config.BlogSettings);

            _metaHelper = new BlogSyncServiceMetaHelper(config);
            _postsHelper = new BlogSyncServicePostsHelper(config);
        }

        public BlogSyncConfiguration Config { get; }

        public async Task<IReadOnlyList<BlogSyncResult>> Update(params BlogKey[] blogKeys)
        {
            if (blogKeys == null)
                throw new ArgumentNullException(nameof(blogKeys));

            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var result = await UpdateBlogs(ensuredBlogKeys, false);
            return result;
        }

        public async Task<IReadOnlyList<BlogSyncResult>> UpdateAll(params BlogKey[] blogKeys)
        {
            if (blogKeys == null)
                throw new ArgumentNullException(nameof(blogKeys));

            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var result = await UpdateBlogs(ensuredBlogKeys, true);
            return result;
        }

        private async Task<BlogSyncResult> UpdateBlog(string blogKey, bool forceShouldUpdate)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = new BlogSyncResult(blogKey);

            var lockKey = blogKey.ToLowerInvariant();

            try
            {
                var blogSetting = _blogSettings.GetBlogSetting(blogKey);

                await UpdateBlogLocker.RunWithLock(
                    lockKey,
                    async () =>
                    {
                        var lastUpdatedAt = !forceShouldUpdate
                                                ? await Config.DataStorage.GetLastUpdatedAt(blogSetting)
                                                : null;

                        await UpdateBlogData(blogSetting, lastUpdatedAt, result);
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
            var updateMetaTask = _metaHelper.Update(blogSetting, lastUpdatedAt);
            var updatePostsTask = _postsHelper.Update(blogSetting, lastUpdatedAt);

            await Task.WhenAll(updateMetaTask, updatePostsTask);

            var meta = updateMetaTask.Result;
            var changeSet = updatePostsTask.Result;

            result.OnDataUpdated(meta, changeSet);

            var saveBlogMetaTask =
                meta != null ? Config.DataStorage.SaveBlogMeta(blogSetting, meta) : TaskHelper.CompletedTask;
            var saveChangesTask = changeSet != null
                                      ? Config.DataStorage.SaveChanges(blogSetting, changeSet)
                                      : TaskHelper.CompletedTask;

            await Task.WhenAll(saveBlogMetaTask, saveChangesTask);
        }

        private async Task<IReadOnlyList<BlogSyncResult>> UpdateBlogs(
            IEnumerable<string> blogKeys,
            bool forceShouldUpdate)
        {
            var resultTasks = blogKeys.Select(async blogKey => await UpdateBlog(blogKey, forceShouldUpdate)).ToList();

            await Task.WhenAll(resultTasks);

            return resultTasks.Select(x => x.Result).ToReadOnlyList();
        }
    }
}
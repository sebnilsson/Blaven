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
        private static readonly ICollection<string> IsUpdatingBlogKeys =
            new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly KeyLocker TryUpdateBlogLocker = new KeyLocker();

        private static readonly KeyLocker UpdateBlogLocker = new KeyLocker();

        private readonly BlogSettingsManager blogSettings;

        private readonly BlogSyncConfiguration config;

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

            this.config = config;

            this.blogSettings = new BlogSettingsManager(config.BlogSettings);
        }

        public IBlogSource BlogSource => this.config.BlogSource;

        public IDataStorage DataStorage => this.config.DataStorage;

        public IDataCacheHandler DataCacheHandler => this.config.DataCacheHandler;

        public IList<BlogSetting> BlogSettings => this.config.BlogSettings;

        public IReadOnlyList<BlogSyncResult> ForceUpdate(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys).ToArray();

            var now = DateTime.Now;

            var result = this.ForceUpdateInternal(now, ensuredBlogKeys);
            return result;
        }

        public bool IsUpdated(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var now = DateTime.Now;

            bool isUpdated = this.IsUpdatedInternal(ensuredBlogKey, now);
            return isUpdated;
        }

        public bool ShouldUpdate(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var now = DateTime.Now;

            bool shouldUpdate = this.ShouldUpdateInternal(ensuredBlogKey, now);
            return shouldUpdate;
        }

        public IReadOnlyList<BlogSyncResult> TryUpdate(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var now = DateTime.Now;

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys).ToArray();

            var result = this.TryUpdateInternal(now, ensuredBlogKeys);
            return result;
        }

        internal IReadOnlyList<BlogSyncResult> ForceUpdateInternal(DateTime now, params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var results = blogKeys.AsParallel().Select(
                blogKey =>
                    {
                        var result = new BlogSyncResult(blogKey);

                        this.UpdateBlog(blogKey, now, result, lastUpdatedAt: null);

                        result.HandleDone();

                        return result;
                    }).ToReadOnlyList();
            return results;
        }

        internal bool IsUpdatedInternal(string blogKey, DateTime now)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            bool isUpdated = this.config.DataCacheHandler.IsUpdated(blogKey, now);
            return isUpdated;
        }

        internal bool ShouldUpdateInternal(string blogKey, DateTime now)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            bool isUpdating = IsUpdating(blogKey);
            if (isUpdating)
            {
                return false;
            }

            bool isUpdated = this.IsUpdatedInternal(blogKey, now);
            return !isUpdated;
        }

        internal IReadOnlyList<BlogSyncResult> TryUpdateInternal(DateTime now, params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var results = blogKeys.AsParallel().Select(
                blogKey =>
                    {
                        var result = new BlogSyncResult(blogKey);

                        this.TryUpdateBlog(blogKey, now, result);

                        result.HandleDone();

                        return result;
                    }).ToReadOnlyList();
            return results;
        }

        private static bool IsUpdating(string blogKey)
        {
            lock (IsUpdatingBlogKeys)
            {
                bool isRunning = IsUpdatingBlogKeys.Contains(blogKey);
                return isRunning;
            }
        }

        private BlogSetting GetBlogSetting(string blogKey)
        {
            var blogSetting = this.config.TryGetBlogSetting(blogKey);
            if (blogSetting == null)
            {
                string message =
                    $"{this.config.GetType().Name} did not contain any {nameof(BlogSetting)} with {nameof(blogKey)} '{blogKey}'.";
                throw new ArgumentOutOfRangeException(nameof(blogKey), message);
            }

            return blogSetting;
        }

        private void TryUpdateBlog(string blogKey, DateTime now, BlogSyncResult result)
        {
            TryUpdateBlogLocker.RunWithLock(
                blogKey.ToLowerInvariant(),
                () =>
                    {
                        bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
                        if (!shouldUpdate)
                        {
                            return;
                        }

                        var blogSetting = this.GetBlogSetting(blogKey);

                        var lastUpdatedAt = this.DataStorage.GetLastPostUpdatedAt(blogSetting);

                        this.UpdateBlog(blogKey, now, result, lastUpdatedAt);
                    });
        }

        private void UpdateBlog(string blogKey, DateTime now, BlogSyncResult result, DateTime? lastUpdatedAt)
        {
            UpdateBlogLocker.RunWithLock(
                blogKey.ToLowerInvariant(),
                () =>
                    {
                        try
                        {
                            lock (IsUpdatingBlogKeys)
                            {
                                IsUpdatingBlogKeys.Add(blogKey);
                            }

                            var blogSetting = this.GetBlogSetting(blogKey);

                            this.UpdateBlogData(blogSetting, lastUpdatedAt, result);

                            this.config.DataCacheHandler.OnUpdated(blogKey, now);
                        }
                        finally
                        {
                            lock (IsUpdatingBlogKeys)
                            {
                                if (IsUpdatingBlogKeys.Contains(blogKey))
                                {
                                    IsUpdatingBlogKeys.Remove(blogKey);
                                }
                            }
                        }
                    });
        }

        private void UpdateBlogData(BlogSetting blogSetting, DateTime? lastUpdatedAt, BlogSyncResult result)
        {
            Parallel.Invoke(
                () =>
                    {
                        result.BlogMeta = BlogSyncServiceUpdateMetaHelper.Update(blogSetting, lastUpdatedAt, this.config);
                    },
                () =>
                    {
                        result.ChangeSet = BlogSyncServiceUpdatePostsHelper.Update(
                            blogSetting,
                            lastUpdatedAt,
                            this.config);
                    });
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
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

        private readonly BlogSyncConfiguration config;

        public BlogSyncService(IBlogSource blogSource, IDataStorage dataStorage, IEnumerable<BlogSetting> blogSettings)
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

            var now = DateTime.Now;

            var result = this.ForceUpdateInternal(now, blogKeys);
            return result;
        }

        public bool IsUpdated(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var now = DateTime.Now;

            bool isUpdated = this.IsUpdatedInternal(blogKey, now);
            return isUpdated;
        }

        public bool ShouldUpdate(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var now = DateTime.Now;

            bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
            return shouldUpdate;
        }

        public IReadOnlyList<BlogSyncResult> TryUpdate(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var now = DateTime.Now;

            var result = this.TryUpdateInternal(now, blogKeys);
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

                        this.UpdateBlog(blogKey, now, lastUpdatedAt: DateTime.MinValue);

                        result.HandleDone(isUpdated: true);

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

                        bool isUpdated = this.TryUpdateBlog(blogKey, now);

                        result.HandleDone(isUpdated);

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

        private bool TryUpdateBlog(string blogKey, DateTime now)
        {
            bool isUpdated = TryUpdateBlogLocker.RunWithLock(
                blogKey.ToLowerInvariant(),
                () =>
                    {
                        bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
                        if (!shouldUpdate)
                        {
                            return false;
                        }

                        this.UpdateBlog(blogKey, now, lastUpdatedAt: now);

                        return true;
                    });
            return isUpdated;
        }

        private void UpdateBlog(string blogKey, DateTime now, DateTime lastUpdatedAt)
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

                            this.UpdateBlogData(blogSetting, lastUpdatedAt);

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

        private void UpdateBlogData(BlogSetting blogSetting, DateTime lastUpdatedAt)
        {
            Parallel.Invoke(
                () => BlogSyncServiceUpdateMetaHelper.Update(blogSetting, lastUpdatedAt, this.config),
                () => BlogSyncServiceUpdatePostsHelper.Update(blogSetting, lastUpdatedAt, this.config));
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
            if (blogSettings == null)
            {
                throw new ArgumentNullException(nameof(blogSettings));
            }

            var config = new BlogSyncConfiguration(
                blogSource,
                dataStorage,
                dataCacheHandler: null,
                blavenIdProvider: null,
                slugProvider: null,
                transformersProvider: null,
                blogSettings: blogSettings);
            return config;
        }
    }
}
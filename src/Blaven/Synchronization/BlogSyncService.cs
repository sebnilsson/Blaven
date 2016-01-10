using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;

namespace Blaven.Synchronization
{
    public class BlogSyncService
    {
        private static readonly ICollection<string> IsUpdatingBlogKeys = new HashSet<string>();

        private static readonly KeyLocker TryUpdateLocker = new KeyLocker();

        private static readonly KeyLocker UpdateAllBlogDataLocker = new KeyLocker();

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
        }

        public IBlogSource BlogSource => this.config.BlogSource;

        public IDataStorage DataStorage => this.config.DataStorage;

        public IDataCacheHandler DataCacheHandler => this.config.DataCacheHandler;

        public IList<BlogSetting> BlogSettings => this.config.BlogSettings;

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

        public IReadOnlyCollection<string> TryUpdate(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var now = DateTime.Now;

            var updatedBlogKeys = this.TryUpdateInternal(now, blogKeys);
            return updatedBlogKeys;
        }

        public void Update(params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var now = DateTime.Now;
            this.UpdateInternal(now, blogKeys);
        }

        internal bool IsUpdatedInternal(string blogKey, DateTime now)
        {
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

        internal IReadOnlyCollection<string> TryUpdateInternal(DateTime now, params string[] blogKeys)
        {
            var updatedBlogKeys = new List<string>();

            Parallel.ForEach(
                blogKeys,
                blogKey =>
                    {
                        bool isUpdated = this.TryUpdateBlog(blogKey, now);

                        if (isUpdated)
                        {
                            updatedBlogKeys.Add(blogKey);
                        }
                    });

            return new ReadOnlyCollection<string>(updatedBlogKeys);
        }

        internal void UpdateInternal(DateTime now, params string[] blogKeys)
        {
            Parallel.ForEach(blogKeys, blogKey => this.UpdateBlog(blogKey, now));
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
            bool isUpdated = TryUpdateLocker.RunWithLock(
                blogKey.ToLowerInvariant(),
                () =>
                    {
                        bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
                        if (!shouldUpdate)
                        {
                            return false;
                        }

                        this.UpdateBlog(blogKey, now);

                        return true;
                    });

            return isUpdated;
        }

        private void UpdateBlog(string blogKey, DateTime now)
        {
            try
            {
                lock (IsUpdatingBlogKeys)
                {
                    IsUpdatingBlogKeys.Add(blogKey);
                }

                var blogSetting = this.GetBlogSetting(blogKey);

                this.UpdateAllBlogData(blogSetting);

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
        }

        private void UpdateAllBlogData(BlogSetting blogSetting)
        {
            UpdateAllBlogDataLocker.RunWithLock(
                blogSetting.BlogKey.ToLowerInvariant(),
                () =>
                    {
                        Parallel.Invoke(
                            () => BlogSyncServiceUpdatePostsHelper.Update(blogSetting, this.config),
                            () => BlogSyncServiceUpdateMetaHelper.Update(blogSetting, this.config));
                    });
        }

        private static bool IsUpdating(string blogKey)
        {
            lock (IsUpdatingBlogKeys)
            {
                bool isRunning = IsUpdatingBlogKeys.Contains(blogKey);
                return isRunning;
            }
        }

        private static BlogSyncConfiguration GetConfig(
            IBlogSource blogSource,
            IDataStorage dataStorage,
            params BlogSetting[] blogSettings)
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
                blogSettings: blogSettings);
            return config;
        }
    }
}
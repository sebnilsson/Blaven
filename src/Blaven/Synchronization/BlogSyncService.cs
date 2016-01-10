using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;

namespace Blaven.Synchronization
{
    public class BlogSyncService
    {
        private static readonly KeyLocker Locker = new KeyLocker();

        private readonly BlogSyncConfiguration config;

        internal static readonly ICollection<string> IsRunningBlogKeys = new HashSet<string>();

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
            bool isRunning = this.IsRunning(blogKey);
            if (isRunning)
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
                        bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
                        if (!shouldUpdate)
                        {
                            return;
                        }

                        this.UpdateInternal(now, blogKeys);

                        updatedBlogKeys.Add(blogKey);
                    });

            return updatedBlogKeys;
        }

        internal void UpdateInternal(DateTime now, params string[] blogKeys)
        {
            Parallel.ForEach(
                blogKeys,
                blogKey =>
                    {
                        Locker.RunWithLock(blogKey, () => this.UpdateBlog(now, blogKey));
                    });
        }

        private bool IsRunning(string blogKey)
        {
            lock (IsRunningBlogKeys)
            {
                bool isRunning = IsRunningBlogKeys.Contains(blogKey);
                return isRunning;
            }
        }

        private void UpdateBlog(DateTime now, string blogKey)
        {
            try
            {
                lock (IsRunningBlogKeys)
                {
                    IsRunningBlogKeys.Add(blogKey);
                }

                var blogSetting = this.GetBlogSetting(blogKey);

                this.UpdateBlogData(blogSetting);

                this.config.DataCacheHandler.OnUpdated(blogKey, now);
            }
            finally
            {
                lock (IsRunningBlogKeys)
                {
                    if (IsRunningBlogKeys.Contains(blogKey))
                    {
                        IsRunningBlogKeys.Remove(blogKey);
                    }
                }
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

        private void UpdateBlogData(BlogSetting blogSetting)
        {
            Parallel.Invoke(
                () => BlogSyncServiceBlogPostsHelper.Update(blogSetting, this.config),
                () => BlogSyncServiceBlogMetaHelper.Update(blogSetting, this.config));
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
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

        internal readonly ICollection<string> IsRunningBlogKeys = new HashSet<string>();

        public BlogSyncService(IBlogSource blogSource, IDataStorage dataStorage)
            : this(GetConfig(blogSource, dataStorage))
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
            lock (this.IsRunningBlogKeys)
            {
                bool isRunning = this.IsRunningBlogKeys.Contains(blogKey);
                return isRunning;
            }
        }

        private void UpdateBlog(DateTime now, string blogKey)
        {
            try
            {
                lock (this.IsRunningBlogKeys)
                {
                    this.IsRunningBlogKeys.Add(blogKey);
                }

                this.UpdateBlogData(blogKey);

                this.config.DataCacheHandler.OnUpdated(blogKey, now);
            }
            finally
            {
                lock (this.IsRunningBlogKeys)
                {
                    if (this.IsRunningBlogKeys.Contains(blogKey))
                    {
                        this.IsRunningBlogKeys.Remove(blogKey);
                    }
                }
            }
        }

        private void UpdateBlogData(string blogKey)
        {
            Parallel.Invoke(
                () => BlogSyncServiceBlogPostsHelper.Update(blogKey, this.config),
                () => BlogSyncServiceBlogMetaHelper.Update(blogKey, this.config));
        }

        private static BlogSyncConfiguration GetConfig(IBlogSource blogSource, IDataStorage dataStorage)
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
                blavenIdProvider: null,
                slugProvider: null);
            return config;
        }
    }
}
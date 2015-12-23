using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Blaven.BlogSources;
using Blaven.Data;

namespace Blaven.Synchronization
{
    public class SynchronizationService
    {
        private static readonly KeyLocker Locker = new KeyLocker();

        private readonly SynchronizationConfiguration config;

        internal readonly ICollection<string> IsRunningBlogKeys = new HashSet<string>();

        public SynchronizationService(IBlogSource dataSource, IDataStorage dataStorage)
            : this(dataSource, dataStorage, SynchronizationConfigurationDefaults.DataCacheHandler.Value)
        {
        }

        public SynchronizationService(
            IBlogSource dataSource,
            IDataStorage dataStorage,
            IDataCacheHandler dataCacheHandler,
            IBlogPostBlavenIdProvider blavenIdProvider = null,
            IBlogPostUrlSlugProvider slugProvider = null)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }
            if (dataStorage == null)
            {
                throw new ArgumentNullException(nameof(dataStorage));
            }

            this.config = new SynchronizationConfiguration(
                dataSource,
                dataStorage,
                dataCacheHandler,
                blavenIdProvider,
                slugProvider);
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

        public bool TryUpdate(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var now = DateTime.Now;

            bool hasUpdated = this.TryUpdateInternal(blogKey, now);
            return hasUpdated;
        }

        public void Update(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var now = DateTime.Now;

            this.UpdateInternal(blogKey, now);
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

        internal bool TryUpdateInternal(string blogKey, DateTime now)
        {
            bool hasUpdated = Locker.RunWithLock(
                blogKey,
                () =>
                    {
                        bool shouldUpdate = this.ShouldUpdateInternal(blogKey, now);
                        if (!shouldUpdate)
                        {
                            return false;
                        }

                        this.UpdateInternal(blogKey, now);

                        return true;
                    });
            return hasUpdated;
        }

        internal void UpdateInternal(string blogKey, DateTime now)
        {
            try
            {
                lock (this.IsRunningBlogKeys)
                {
                    this.IsRunningBlogKeys.Add(blogKey);
                }

                this.UpdateInternal(blogKey);

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

        private void UpdateInternal(string blogKey)
        {
            Parallel.Invoke(
                () => SynchronizationServiceBlogPostsHelper.Update(blogKey, this.config),
                () => SynchronizationServiceBlogMetaHelper.Update(blogKey, this.config));
        }

        private bool IsRunning(string blogKey)
        {
            lock (this.IsRunningBlogKeys)
            {
                bool isRunning = this.IsRunningBlogKeys.Contains(blogKey);
                return isRunning;
            }
        }
    }
}
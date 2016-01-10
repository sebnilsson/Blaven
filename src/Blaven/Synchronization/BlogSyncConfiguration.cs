using System;

using Blaven.BlogSources;
using Blaven.Data;

namespace Blaven.Synchronization
{
    public class BlogSyncConfiguration
    {
        public BlogSyncConfiguration(
            IBlogSource blogSource,
            IDataStorage dataStorage,
            IDataCacheHandler dataCacheHandler,
            IBlogPostBlavenIdProvider blavenIdProvider,
            IBlogPostUrlSlugProvider slugProvider)
        {
            if (blogSource == null)
            {
                throw new ArgumentNullException(nameof(blogSource));
            }
            if (dataStorage == null)
            {
                throw new ArgumentNullException(nameof(dataStorage));
            }

            this.BlogSource = blogSource;
            this.DataStorage = dataStorage;

            this.DataCacheHandler = dataCacheHandler ?? BlogSyncConfigurationDefaults.DataCacheHandler.Value;
            this.BlavenIdProvider = blavenIdProvider ?? BlogSyncConfigurationDefaults.BlavenIdProvider.Value;
            this.SlugProvider = slugProvider ?? BlogSyncConfigurationDefaults.SlugProvider.Value;
        }

        public IBlogPostBlavenIdProvider BlavenIdProvider { get; private set; }

        public IBlogSource BlogSource { get; private set; }

        public IDataCacheHandler DataCacheHandler { get; private set; }

        public IDataStorage DataStorage { get; private set; }

        public IBlogPostUrlSlugProvider SlugProvider { get; private set; }
    }
}
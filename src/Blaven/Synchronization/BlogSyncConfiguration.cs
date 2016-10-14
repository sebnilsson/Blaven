using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.BlogSources;
using Blaven.Data;
using Blaven.Transformers;

namespace Blaven.Synchronization
{
    public class BlogSyncConfiguration
    {
        public BlogSyncConfiguration(
            IBlogSource blogSource,
            IDataStorage dataStorage,
            IBlogPostBlavenIdProvider blavenIdProvider = null,
            IBlogPostUrlSlugProvider slugProvider = null,
            IDataCacheHandler dataCacheHandler = null,
            BlogTransformersProvider transformersProvider = null,
            IEnumerable<BlogSetting> blogSettings = null)
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

            this.BlavenIdProvider = blavenIdProvider ?? BlogSyncConfigurationDefaults.BlavenIdProvider;
            this.SlugProvider = slugProvider ?? BlogSyncConfigurationDefaults.SlugProvider;
            this.DataCacheHandler = dataCacheHandler ?? BlogSyncConfigurationDefaults.DataCacheHandler;
            this.TransformersProvider = transformersProvider ?? BlogSyncConfigurationDefaults.TransformersProvider;
            this.BlogSettings = (blogSettings ?? new BlogSetting[0]).Where(x => x.BlogKey != null).ToList();
        }

        public IBlogPostBlavenIdProvider BlavenIdProvider { get; private set; }

        public IList<BlogSetting> BlogSettings { get; }

        public IBlogSource BlogSource { get; private set; }

        public IDataCacheHandler DataCacheHandler { get; private set; }

        public IDataStorage DataStorage { get; private set; }

        public IBlogPostUrlSlugProvider SlugProvider { get; private set; }

        public BlogTransformersProvider TransformersProvider { get; private set; }

        public BlogSetting TryGetBlogSetting(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var blogSetting =
                this.BlogSettings.FirstOrDefault(
                    x => x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase));
            return blogSetting;
        }
    }
}
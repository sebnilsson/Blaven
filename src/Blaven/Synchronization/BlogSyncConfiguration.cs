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
            this.TransformersProvider = transformersProvider ?? BlogSyncConfigurationDefaults.TransformersProvider;
            this.BlogSettings = (blogSettings ?? new BlogSetting[0]).Where(x => x.BlogKey != null).ToList();
        }

        public IBlogPostBlavenIdProvider BlavenIdProvider { get; private set; }

        public IList<BlogSetting> BlogSettings { get; }

        public IBlogSource BlogSource { get; private set; }

        public IDataStorage DataStorage { get; private set; }

        public IBlogPostUrlSlugProvider SlugProvider { get; private set; }

        public BlogTransformersProvider TransformersProvider { get; private set; }

        

        public static BlogSyncConfiguration Create(
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
                             slugProvider: null,
                             blavenIdProvider: null,
                             transformersProvider: null,
                             blogSettings: blogSettings ?? Enumerable.Empty<BlogSetting>());
            return config;
        }
    }
}
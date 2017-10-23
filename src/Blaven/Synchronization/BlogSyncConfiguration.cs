using System;
using System.Collections.Generic;
using System.Linq;
using Blaven.BlogSources;
using Blaven.DataStorage;
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
            BlogSource = blogSource ?? throw new ArgumentNullException(nameof(blogSource));
            DataStorage = dataStorage ?? throw new ArgumentNullException(nameof(dataStorage));

            BlavenIdProvider = blavenIdProvider ?? BlogSyncConfigurationDefaults.BlavenIdProvider;
            SlugProvider = slugProvider ?? BlogSyncConfigurationDefaults.SlugProvider;
            TransformersProvider = transformersProvider ?? BlogSyncConfigurationDefaults.TransformersProvider;
            BlogSettings = (blogSettings ?? new BlogSetting[0]).Where(x => x.BlogKey != null).ToList();
        }

        public IBlogPostBlavenIdProvider BlavenIdProvider { get; }

        public IList<BlogSetting> BlogSettings { get; }

        public IBlogSource BlogSource { get; }

        public IDataStorage DataStorage { get; }

        public IBlogPostUrlSlugProvider SlugProvider { get; }

        public BlogTransformersProvider TransformersProvider { get; }

        internal static BlogSyncConfiguration Create(
            IBlogSource blogSource,
            IDataStorage dataStorage,
            IEnumerable<BlogSetting> blogSettings)
        {
            if (blogSource == null)
                throw new ArgumentNullException(nameof(blogSource));
            if (dataStorage == null)
                throw new ArgumentNullException(nameof(dataStorage));

            var config = new BlogSyncConfiguration(
                blogSource,
                dataStorage,
                slugProvider: null,
                blavenIdProvider: null,
                transformersProvider: null,
                blogSettings: blogSettings);
            return config;
        }
    }
}
using System;

using Blaven.Data;

namespace Blaven.Synchronization
{
    internal static class BlogSyncConfigurationDefaults
    {
        internal static readonly Lazy<IDataCacheHandler> DataCacheHandler =
            new Lazy<IDataCacheHandler>(() => new MemoryDataCacheHandler());

        internal static readonly Lazy<IBlogPostBlavenIdProvider> BlavenIdProvider =
            new Lazy<IBlogPostBlavenIdProvider>(() => new BlavenBlogPostBlavenIdProvider());

        internal static readonly Lazy<IBlogPostUrlSlugProvider> SlugProvider =
            new Lazy<IBlogPostUrlSlugProvider>(() => new BlavenBlogPostUrlSlugProvider());
    }
}
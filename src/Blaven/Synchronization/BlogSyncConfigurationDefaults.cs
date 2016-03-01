﻿using System;

using Blaven.Data;
using Blaven.Transformers;

namespace Blaven.Synchronization
{
    internal static class BlogSyncConfigurationDefaults
    {
        private static readonly Lazy<IBlogPostBlavenIdProvider> BlavenIdProviderLazy =
            new Lazy<IBlogPostBlavenIdProvider>(
                () =>
                new PermalinkBlogPostBlavenIdProvider(includePublishedYearAndMonth: true, includePublishedDay: false));

        private static readonly Lazy<IDataCacheHandler> DataCacheHandlerLazy =
            new Lazy<IDataCacheHandler>(() => new MemoryDataCacheHandler());

        private static readonly Lazy<IBlogPostUrlSlugProvider> SlugProviderLazy =
            new Lazy<IBlogPostUrlSlugProvider>(() => new BlavenBlogPostUrlSlugProvider());

        private static readonly Lazy<BlogTransformersProvider> TransformersProviderLazy =
            new Lazy<BlogTransformersProvider>(
                () => new BlogTransformersProvider(BlogTransformersProvider.GetDefaultTransformers()));

        public static IBlogPostBlavenIdProvider BlavenIdProvider => BlavenIdProviderLazy.Value;

        public static IDataCacheHandler DataCacheHandler => DataCacheHandlerLazy.Value;

        public static IBlogPostUrlSlugProvider SlugProvider => SlugProviderLazy.Value;

        public static BlogTransformersProvider TransformersProvider => TransformersProviderLazy.Value;
    }
}
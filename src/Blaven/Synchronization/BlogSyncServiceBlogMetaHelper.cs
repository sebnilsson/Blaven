﻿using System;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceBlogMetaHelper
    {
        public static BlogMeta Update(BlogSetting blogSetting, BlogSyncConfiguration config)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var meta = config.BlogSource.GetMeta(blogSetting);
            if (meta == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetMeta)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            config.DataStorage.SaveBlogMeta(blogSetting, meta);

            return meta;
        }
    }
}
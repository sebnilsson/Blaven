using System;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceUpdateMetaHelper
    {
        public static BlogMeta Update(BlogSetting blogSetting, DateTime? lastUpdatedAt, BlogSyncConfiguration config)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var meta = config.BlogSource.GetMeta(blogSetting, lastUpdatedAt);
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
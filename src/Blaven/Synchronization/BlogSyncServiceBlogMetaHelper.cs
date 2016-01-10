using System;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceBlogMetaHelper
    {
        public static BlogMeta Update(string blogKey, BlogSyncConfiguration config)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var meta = config.BlogSource.GetMeta(blogKey);
            if (meta == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetMeta)} for {nameof(blogKey)} '{blogKey}'.";
                throw new BlogSyncException(message);
            }

            config.DataStorage.SaveBlogMeta(blogKey, meta);

            return meta;
        }
    }
}
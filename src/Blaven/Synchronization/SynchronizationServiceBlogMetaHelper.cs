using System;

namespace Blaven.Synchronization
{
    internal static class SynchronizationServiceBlogMetaHelper
    {
        public static BlogMeta Update(string blogKey, SynchronizationConfiguration config)
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
                throw new SynchronizationException(message);
            }

            config.DataStorage.SaveBlogMeta(blogKey, meta);

            return meta;
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceUpdateMetaHelper
    {
        public static async Task<BlogMeta> Update(
            BlogSetting blogSetting,
            DateTime? lastUpdatedAt,
            BlogSyncConfiguration config)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var meta = await config.BlogSource.GetMeta(blogSetting, lastUpdatedAt);
            if (meta == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetMeta)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            return meta;
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceMetaHelper
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

            BlogMeta meta;
            try
            {
                meta = await config.BlogSource.GetMeta(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                string message =
                    $"{nameof(config.BlogSource)} threw an unexpected excetion from {nameof(config.BlogSource.GetMeta)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncBlogSourceException(message, ex);
            }

            if (meta == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetMeta)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncBlogSourceResultException(message);
            }

            return meta;
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal class BlogSyncServiceMetaHelper
    {
        private readonly BlogSyncConfiguration config;

        public BlogSyncServiceMetaHelper(BlogSyncConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.config = config;
        }

        public async Task<BlogMeta> Update(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            BlogMeta meta;
            try
            {
                meta = await this.config.BlogSource.GetMeta(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                string message =
                    $"{nameof(this.config.BlogSource)} threw an unexpected excetion from {nameof(this.config.BlogSource.GetMeta)} "
                    + $"for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncBlogSourceException(message, ex);
            }

            //if (meta == null)
            //{
            //    string message =
            //        $"{nameof(this.config.BlogSource)} returned a null result from {nameof(this.config.BlogSource.GetMeta)} "
            //        + $"for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
            //    throw new BlogSyncBlogSourceResultException(message);
            //}

            return meta;
        }
    }
}
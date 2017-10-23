using System;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal class BlogSyncServiceMetaHelper
    {
        private readonly BlogSyncConfiguration _config;

        public BlogSyncServiceMetaHelper(BlogSyncConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<BlogMeta> Update(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));

            BlogMeta meta;
            try
            {
                meta = await _config.BlogSource.GetMeta(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                var message =
                    $"{nameof(_config.BlogSource)} threw an unexpected excetion from {nameof(_config.BlogSource.GetMeta)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncBlogSourceException(message, ex);
            }

            //if (meta == null)
            //{
            //    string message =
            //        $"{nameof(this.config.BlogSource)} returned a null result from {nameof(this.config.BlogSource.GetMeta)}"
            //        + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
            //    throw new BlogSyncBlogSourceResultException(message);
            //}

            return meta;
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceUpdatePostsHelper
    {
        public static async Task<BlogSourceChangeSet> Update(
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

            var existingPosts = await config.DataStorage.GetPostBases(blogSetting, lastUpdatedAt);
            if (existingPosts == null)
            {
                string message =
                    $"{nameof(config.DataStorage)} returned a null result from {nameof(config.DataStorage.GetPostBases)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            var changeSet = await config.BlogSource.GetChanges(blogSetting, existingPosts, lastUpdatedAt);
            if (changeSet == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetChanges)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            HandleChanges(blogSetting, changeSet, config);

            return changeSet;
        }

        private static void HandleChanges(
            BlogSetting blogSetting,
            BlogSourceChangeSet sourceChanges,
            BlogSyncConfiguration config)
        {
            var posts = sourceChanges.InsertedBlogPosts.Concat(sourceChanges.UpdatedBlogPosts).ToList();

            foreach (var post in posts)
            {
                post.BlogKey = blogSetting.BlogKey;

                post.UrlSlug = config.SlugProvider.GetUrlSlug(post);
                post.BlavenId = config.BlavenIdProvider.GetBlavenId(post);

                config.TransformersProvider.ApplyTransformers(post);
            }
        }
    }
}
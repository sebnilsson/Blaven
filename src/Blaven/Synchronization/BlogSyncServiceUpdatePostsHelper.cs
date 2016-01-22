using System;

using Blaven.BlogSources;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServiceUpdatePostsHelper
    {
        public static BlogSourceChangeSet Update(BlogSetting blogSetting, BlogSyncConfiguration config)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var dbBlogPosts = config.DataStorage.GetPostBases(blogSetting);
            if (dbBlogPosts == null)
            {
                string message =
                    $"{nameof(config.DataStorage)} returned a null result from {nameof(config.DataStorage.GetPostBases)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            var sourceChanges = config.BlogSource.GetChanges(blogSetting, dbBlogPosts);
            if (sourceChanges == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetChanges)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncException(message);
            }

            HandleChanges(blogSetting, sourceChanges, config);

            config.DataStorage.SaveChanges(blogSetting, sourceChanges);

            return sourceChanges;
        }

        private static void HandleChanges(
            BlogSetting blogSetting,
            BlogSourceChangeSet sourceChanges,
            BlogSyncConfiguration config)
        {
            foreach (var post in sourceChanges.InsertedBlogPosts)
            {
                post.BlogKey = blogSetting.BlogKey;

                post.BlavenId = config.BlavenIdProvider.GetId(post);
                post.UrlSlug = config.SlugProvider.GetSlug(post);

                config.TransformersProvider.ApplyTransformers(post);
            }
        }
    }
}
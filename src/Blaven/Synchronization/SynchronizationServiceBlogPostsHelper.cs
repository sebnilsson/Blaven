using System;

using Blaven.BlogSources;

namespace Blaven.Synchronization
{
    internal static class SynchronizationServiceBlogPostsHelper
    {
        public static BlogSourceChangeSet Update(string blogKey, SynchronizationConfiguration config)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var dbBlogPosts = config.DataStorage.GetBlogPosts(blogKey);
            if (dbBlogPosts == null)
            {
                string message =
                    $"{nameof(config.DataStorage)} returned a null result from {nameof(config.DataStorage.GetBlogPosts)} for {nameof(blogKey)} '{blogKey}'.";
                throw new SynchronizationException(message);
            }

            var sourceChanges = config.BlogSource.GetChanges(blogKey, dbBlogPosts);
            if (sourceChanges == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetChanges)} for {nameof(blogKey)} '{blogKey}'.";
                throw new SynchronizationException(message);
            }

            HandleChanges(blogKey, sourceChanges, config);

            config.DataStorage.SaveChanges(blogKey, sourceChanges);

            return sourceChanges;
        }

        private static void HandleChanges(
            string blogKey,
            BlogSourceChangeSet sourceChanges,
            SynchronizationConfiguration config)
        {
            foreach (var post in sourceChanges.InsertedBlogPosts)
            {
                post.BlogKey = blogKey;

                post.BlavenId = config.BlavenIdProvider.GetId(post);
                post.UrlSlug = config.SlugProvider.GetSlug(post);
            }
        }
    }
}
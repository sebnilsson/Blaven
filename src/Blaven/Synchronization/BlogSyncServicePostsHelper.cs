using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.BlogSources;

namespace Blaven.Synchronization
{
    internal static class BlogSyncServicePostsHelper
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

            IReadOnlyList<BlogPostBase> dataStoragePosts;
            try
            {
                dataStoragePosts = await config.DataStorage.GetPostBases(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                string message =
                    $"{nameof(config.DataStorage)} threw an unexpected excetion from {nameof(config.DataStorage.GetPostBases)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncDataStorageException(message, ex);
            }

            if (dataStoragePosts == null)
            {
                string message =
                    $"{nameof(config.DataStorage)} returned a null result from {nameof(config.DataStorage.GetPostBases)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncDataStorageResultException(message);
            }
            
            IReadOnlyList<BlogPost> sourcePosts;
            try
            {
                sourcePosts = await config.BlogSource.GetBlogPosts(blogSetting, dataStoragePosts, lastUpdatedAt);
            }
            catch (Exception ex)
            {

                string message =
                    $"{nameof(config.BlogSource)} threw an unexpected excetion from {nameof(config.BlogSource.GetBlogPosts)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncBlogSourceException(message, ex);
            }

            if (sourcePosts == null)
            {
                string message =
                    $"{nameof(config.BlogSource)} returned a null result from {nameof(config.BlogSource.GetBlogPosts)} for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncBlogSourceResultException(message);
            }

            sourcePosts = CleanBlogPosts(sourcePosts);

            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(
                blogSetting.BlogKey,
                sourcePosts,
                dataStoragePosts);

            HandleChanges(blogSetting.BlogKey, changeSet, config);

            return changeSet;
        }

        private static IReadOnlyList<BlogPost> CleanBlogPosts(IEnumerable<BlogPost> blogPosts)
        {
            var cleanedBlogPosts = blogPosts.Where(x => x != null)
                    .OrderByDescending(x => x.UpdatedAt)
                    .GroupBy(x => x.SourceId)
                    .Select(x => x.First())
                    .ToReadOnlyList();

            return cleanedBlogPosts;
        }

        private static void HandleChanges(string blogKey, BlogSourceChangeSet changeSet, BlogSyncConfiguration config)
        {
            var posts = changeSet.InsertedBlogPosts.Concat(changeSet.UpdatedBlogPosts).ToList();

            foreach (var post in posts)
            {
                post.BlogKey = blogKey;

                post.UrlSlug = config.SlugProvider.GetUrlSlug(post);
                post.BlavenId = config.BlavenIdProvider.GetBlavenId(post);

                config.TransformersProvider.ApplyTransformers(post);
            }
        }
    }
}
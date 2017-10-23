using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal class BlogSyncServicePostsHelper
    {
        private readonly BlogSyncConfiguration _config;

        public BlogSyncServicePostsHelper(BlogSyncConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<BlogSyncPostsChangeSet> Update(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var sourcePosts = await GetSourcePosts(blogSetting, lastUpdatedAt);
            if (sourcePosts == null)
                return null;

            var dataStoragePosts = await GetDataStoragePosts(blogSetting, lastUpdatedAt);

            var changeSet = BlogSyncPostsChangeSetHelper.GetChangeSet(
                blogSetting.BlogKey,
                sourcePosts,
                dataStoragePosts);

            HandleChanges(blogSetting.BlogKey, changeSet);

            return changeSet;
        }

        private static IReadOnlyList<BlogPost> CleanBlogPosts(IEnumerable<BlogPost> blogPosts)
        {
            var cleanedBlogPosts = blogPosts.Where(x => x != null)
                .OrderByDescending(x => x.UpdatedAt)
                .Distinct(x => x.SourceId)
                .ToReadOnlyList();

            return cleanedBlogPosts;
        }

        private async Task<IReadOnlyList<BlogPostBase>> GetDataStoragePosts(
            BlogSetting blogSetting,
            DateTime? lastUpdatedAt)
        {
            IReadOnlyList<BlogPostBase> dataStoragePosts;
            try
            {
                dataStoragePosts = await _config.DataStorage.GetBlogPosts(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                var message =
                    $"{nameof(_config.DataStorage)} threw an unexpected excetion from {nameof(_config.DataStorage.GetBlogPosts)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncDataStorageException(message, ex);
            }

            if (dataStoragePosts == null)
            {
                var message =
                    $"{nameof(_config.DataStorage)} returned a null result from {nameof(_config.DataStorage.GetBlogPosts)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BlogSyncDataStorageResultException(message);
            }

            return dataStoragePosts;
        }

        private async Task<IReadOnlyList<BlogPost>> GetSourcePosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            IReadOnlyList<BlogPost> sourcePosts;
            try
            {
                sourcePosts = await _config.BlogSource.GetBlogPosts(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                var message =
                    $"{nameof(_config.BlogSource)} threw an unexpected excetion from {nameof(_config.BlogSource.GetBlogPosts)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncBlogSourceException(message, ex);
            }

            //if (sourcePosts == null)
            //{
            //    string message =
            //        $"{nameof(this.config.BlogSource)} returned a null result from {nameof(this.config.BlogSource.GetBlogPosts)}"
            //        + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
            //    throw new BlogSyncBlogSourceResultException(message);
            //}

            if (sourcePosts != null)
                sourcePosts = CleanBlogPosts(sourcePosts);

            return sourcePosts;
        }

        private void HandleChanges(string blogKey, BlogSyncPostsChangeSet changeSet)
        {
            var posts = changeSet.InsertedBlogPosts.Concat(changeSet.UpdatedBlogPosts).ToList();

            foreach (var post in posts)
            {
                post.BlogKey = blogKey;

                post.UrlSlug = _config.SlugProvider.GetUrlSlug(post);
                post.BlavenId = _config.BlavenIdProvider.GetBlavenId(post);

                _config.TransformersProvider.ApplyTransformers(post);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.Synchronization
{
    internal class BlogSyncServicePostsHelper
    {
        private readonly BlogSyncConfiguration config;

        public BlogSyncServicePostsHelper(BlogSyncConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            this.config = config;
        }

        public async Task<BlogSyncChangeSet> Update(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var dataStoragePosts = await this.GetDataStoragePosts(blogSetting, lastUpdatedAt);

            var sourcePosts = await this.GetSourcePosts(blogSetting, lastUpdatedAt, dataStoragePosts);

            var changeSet = BlogSyncChangeSetHelper.GetChangeSet(blogSetting.BlogKey, sourcePosts, dataStoragePosts);

            this.HandleChanges(blogSetting.BlogKey, changeSet);

            return changeSet;
        }

        private static IReadOnlyList<BlogPost> CleanBlogPosts(IEnumerable<BlogPost> blogPosts)
        {
            var cleanedBlogPosts =
                blogPosts.Where(x => x != null)
                    .OrderByDescending(x => x.UpdatedAt)
                    .GroupBy(x => x.SourceId)
                    .Select(x => x.First())
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
                dataStoragePosts = await this.config.DataStorage.GetPostBases(blogSetting, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                string message =
                    $"{nameof(this.config.DataStorage)} threw an unexpected excetion from {nameof(this.config.DataStorage.GetPostBases)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}': {ex.Message.TrimEnd('.')}.";
                throw new BlogSyncDataStorageException(message, ex);
            }

            //if (dataStoragePosts == null)
            //{
            //    string message =
            //        $"{nameof(this.config.DataStorage)} returned a null result from {nameof(this.config.DataStorage.GetPostBases)}"
            //        + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
            //    throw new BlogSyncDataStorageResultException(message);
            //}

            return dataStoragePosts;
        }

        private async Task<IReadOnlyList<BlogPost>> GetSourcePosts(
            BlogSetting blogSetting,
            DateTime? lastUpdatedAt,
            IEnumerable<BlogPostBase> dataStoragePosts)
        {
            IReadOnlyList<BlogPost> sourcePosts;
            try
            {
                sourcePosts = await this.config.BlogSource.GetBlogPosts(blogSetting, dataStoragePosts, lastUpdatedAt);
            }
            catch (Exception ex)
            {
                string message =
                    $"{nameof(this.config.BlogSource)} threw an unexpected excetion from {nameof(this.config.BlogSource.GetBlogPosts)}"
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

            sourcePosts = CleanBlogPosts(sourcePosts);

            return sourcePosts;
        }

        private void HandleChanges(string blogKey, BlogSyncChangeSet changeSet)
        {
            var posts = changeSet.InsertedBlogPosts.Concat(changeSet.UpdatedBlogPosts).ToList();

            foreach (var post in posts)
            {
                post.BlogKey = blogKey;

                post.UrlSlug = this.config.SlugProvider.GetUrlSlug(post);
                post.BlavenId = this.config.BlavenIdProvider.GetBlavenId(post);

                this.config.TransformersProvider.ApplyTransformers(post);
            }
        }
    }
}
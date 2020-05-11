using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blaven.BlogSources;
using Blaven.Storage;
using Blaven.Transformation;

namespace Blaven.Synchronization
{
    public class BlogSyncService : IBlogSyncService
    {
        private readonly IBlogSource _blogSource;
        private readonly IStorageSyncRepository _storageSyncRepo;
        private readonly IBlogPostStorageTransformService _transformService;

        public BlogSyncService(
            IBlogSource blogSource,
            IStorageSyncRepository storageSyncRepo,
            IBlogPostStorageTransformService transformService)
        {
            _blogSource = blogSource
                ?? throw new ArgumentNullException(nameof(blogSource));
            _storageSyncRepo = storageSyncRepo
                ?? throw new ArgumentNullException(nameof(storageSyncRepo));
            _transformService = transformService
                ?? throw new ArgumentNullException(nameof(transformService));
        }

        public async Task<SyncResult> Synchronize(
            BlogKey blogKey = default,
            DateTimeOffset? updatedAfter = null)
        {
            var stopwatch = Stopwatch.StartNew();

            var data =
                await _blogSource.GetData(blogKey, updatedAfter)
                    .ConfigureAwait(false);

            var meta =
                await SyncMeta(data, blogKey, updatedAfter)
                    .ConfigureAwait(false);
            var posts =
                await SyncPosts(data, blogKey, updatedAfter)
                    .ConfigureAwait(false);

            await Update(blogKey, meta, posts, updatedAfter)
                .ConfigureAwait(false);

            stopwatch.Stop();

            return new SyncResult(
                blogKey,
                meta,
                posts,
                stopwatch.Elapsed);
        }

        private async Task<BlogMeta?> SyncMeta(
            BlogSourceData data,
            BlogKey blogKey,
            DateTimeOffset? updatedAfter)
        {
            var blogSourceMeta = data.Meta;

            if (blogSourceMeta == null)
            {
                return null;
            }

            var storageSourceMeta =
                await
                _storageSyncRepo
                    .GetMeta(blogKey, updatedAfter)
                    .ConfigureAwait(false);

            var equals =
                BlogMetaComparer.AreMetasEqual(blogSourceMeta, storageSourceMeta);

            return !equals ? blogSourceMeta : null;
        }

        private async Task<SyncBlogPosts> SyncPosts(
            BlogSourceData data,
            BlogKey blogKey,
            DateTimeOffset? updatedAfter)
        {
            var blogSourcePosts = data.Posts;

            var storagePosts =
                await
                _storageSyncRepo
                    .GetPosts(blogKey, updatedAfter)
                    .ConfigureAwait(false);

            return BlogPostComparer.Compare(blogSourcePosts, storagePosts);
        }

        private async Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            SyncBlogPosts posts,
            DateTimeOffset? updatedAfter)
        {
            foreach (var post in posts.Inserted)
            {
                _transformService.TransformPost(post);
            }
            foreach (var post in posts.Updated)
            {
                _transformService.TransformPost(post);
            }

            await
                _storageSyncRepo.Update(
                    blogKey,
                    updatedAfter,
                    meta,
                    insertedPosts: posts.Inserted,
                    updatedPosts: posts.Updated,
                    deletedPosts: posts.Deleted)
                .ConfigureAwait(false);
        }
    }
}

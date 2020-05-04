using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blaven.BlogSource;
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

            var meta =
                await SyncMeta(blogKey, updatedAfter).ConfigureAwait(false);
            var posts =
                await SyncPosts(blogKey, updatedAfter).ConfigureAwait(false);

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
            BlogKey blogKey,
            DateTimeOffset? updatedAfter)
        {
            var blogSourceMeta =
                await
                _blogSource
                    .GetMeta(blogKey, updatedAfter)
                    .ConfigureAwait(false);

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
            BlogKey blogKey,
            DateTimeOffset? updatedAfter)
        {
            var blogSourcePosts =
                await
                _blogSource
                    .GetPosts(blogKey, updatedAfter)
                    .ConfigureAwait(false);

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
                    meta,
                    insertedPosts: posts.Inserted,
                    updatedPosts: posts.Updated,
                    deletedPosts: posts.Deleted,
                    updatedAfter)
                .ConfigureAwait(false);
        }
    }
}

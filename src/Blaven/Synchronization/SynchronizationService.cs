using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Blaven.BlogSource;
using Blaven.Storage;
using Blaven.Synchronization.Transformation;

namespace Blaven.Synchronization
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly IBlogSource _blogSource;
        private readonly IStorage _storage;
        private readonly ITransformerService _transformerService;

        public SynchronizationService(
            IBlogSource blogSource,
            IStorage storage,
            ITransformerService transformerService)
        {
            _blogSource = blogSource
                ?? throw new ArgumentNullException(nameof(blogSource));
            _storage = storage
                ?? throw new ArgumentNullException(nameof(storage));
            _transformerService = transformerService
                ?? throw new ArgumentNullException(nameof(transformerService));
        }

        public async Task<SynchronizationResult> Synchronize(
            BlogKey blogKey = default,
            DateTimeOffset? lastUpdatedAt = null)
        {
            var stopwatch = Stopwatch.StartNew();

            var meta =
                await SyncMeta(blogKey, lastUpdatedAt).ConfigureAwait(false);
            var posts =
                await SyncPosts(blogKey, lastUpdatedAt).ConfigureAwait(false);

            await Update(blogKey, meta, posts, lastUpdatedAt)
                .ConfigureAwait(false);

            stopwatch.Stop();

            return new SynchronizationResult(
                blogKey,
                meta,
                posts,
                stopwatch.Elapsed);
        }

        private async Task<BlogMeta?> SyncMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var blogSourceMeta =
                await
                _blogSource
                    .GetMeta(blogKey, lastUpdatedAt)
                    .ConfigureAwait(false);

            if (blogSourceMeta == null)
            {
                return null;
            }

            var storageSourceMeta =
                await
                _storage
                    .GetMeta(blogKey, lastUpdatedAt)
                    .ConfigureAwait(false);

            var equals =
                BlogMetaComparer.AreMetasEqual(blogSourceMeta, storageSourceMeta);

            return !equals ? blogSourceMeta : null;
        }

        private async Task<SynchronizationBlogPosts> SyncPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var blogSourcePosts =
                await
                _blogSource
                    .GetPosts(blogKey, lastUpdatedAt)
                    .ConfigureAwait(false);

            var storagePosts =
                await
                _storage
                    .GetPosts(blogKey, lastUpdatedAt)
                    .ConfigureAwait(false);

            return BlogPostComparer.Compare(blogSourcePosts, storagePosts);
        }

        private async Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            SynchronizationBlogPosts posts,
            DateTimeOffset? lastUpdatedAt)
        {
            foreach (var post in posts.Inserted)
            {
                _transformerService.TransformPost(post);
            }
            foreach (var post in posts.Updated)
            {
                _transformerService.TransformPost(post);
            }

            await
                _storage.Update(
                    blogKey,
                    meta,
                    insertedPosts: posts.Inserted,
                    updatedPosts: posts.Updated,
                    deletedPosts: posts.Deleted,
                    lastUpdatedAt)
                .ConfigureAwait(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaven.Storage;

namespace Blaven
{
    public class BlogService : IBlogService
    {
        private readonly IStorageRepository _storageRepository;

        public BlogService(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository
                ?? throw new ArgumentNullException(nameof(storageRepository));
        }

        public async Task<BlogMeta> GetBlogMeta(BlogKey blogKey = default)
        {
            return await
                _storageRepository.GetBlogMeta(blogKey).ConfigureAwait(false);
        }

        public async Task<BlogPost> GetPost(
            string id,
            BlogKey blogKey = default)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            return await
                _storageRepository
                    .GetPost(id, blogKey)
                    .ConfigureAwait(false);
        }

        public async Task<BlogPost> GetPostBySlug(
            string slug,
            BlogKey blogKey = default)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            return await
                _storageRepository
                    .GetPostBySlug(slug, blogKey)
                    .ConfigureAwait(false);
        }

        public async Task<BlogPost> GetPostBySourceId(
            string sourceId,
            BlogKey blogKey = default)
        {
            if (sourceId is null)
                throw new ArgumentNullException(nameof(sourceId));

            return await
                _storageRepository
                    .GetPostBySourceId(sourceId, blogKey)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogArchiveItem>> ListArchive(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListArchive(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogMeta>> ListBlogMetas(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListBlogMetas(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListPostHeaders(paging, blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTime archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListPostsByArchive(archiveDate, paging, blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tag,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListPostsByTag(tag, paging, blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogTagItem>> ListTags(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .ListTags(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _storageRepository
                    .SearchPostHeaders(searchText, paging, blogKeys)
                    .ConfigureAwait(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaven.Storage;

namespace Blaven
{
    public class BlogService : IBlogService
    {
        private readonly IStorageQueryRepository _repository;

        public BlogService(IStorageQueryRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<BlogMeta?> GetMeta(BlogKey blogKey = default)
        {
            return await _repository.GetMeta(blogKey).ConfigureAwait(false);
        }

        public async Task<BlogPost?> GetPost(
            string id,
            BlogKey blogKey = default)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            return await
                _repository
                    .GetPost(id, blogKey)
                    .ConfigureAwait(false);
        }

        public async Task<BlogPost?> GetPostBySlug(
            string slug,
            BlogKey blogKey = default)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            return await
                _repository
                    .GetPostBySlug(slug, blogKey)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListAllDates(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListAllTags(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogMeta>> ListMetas(
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListMetas(blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListPostHeaders(paging, blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListPostsByArchive(archiveDate, paging, blogKeys)
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            return await
                _repository
                    .ListPostsByTag(tagName, paging, blogKeys)
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
                _repository
                    .SearchPostHeaders(searchText, paging, blogKeys)
                    .ConfigureAwait(false);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blaven.Storage;
using Blaven.Transformation;

namespace Blaven
{
    public class BlogQueryService : IBlogQueryService
    {
        private readonly IStorageQueryRepository _repository;
        private readonly IBlogPostQueryTransformService _transformService;

        public BlogQueryService(
            IStorageQueryRepository repository,
            IBlogPostQueryTransformService transformService)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
            _transformService = transformService
                ?? throw new ArgumentNullException(nameof(transformService));
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

            var post = await
                _repository
                    .GetPost(id, blogKey)
                    .ConfigureAwait(false);

            return post?.TryTransformPost(_transformService);
        }

        public async Task<BlogPost?> GetPostBySlug(
            string slug,
            BlogKey blogKey = default)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            var post = await
                _repository
                    .GetPostBySlug(slug, blogKey)
                    .ConfigureAwait(false);

            return post?.TryTransformPost(_transformService);
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

        public async Task<IReadOnlyList<BlogMeta>> ListAllMetas()
        {
            return await
                _repository
                    .ListAllMetas()
                    .ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPosts(
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var postHeaders = await
                _repository
                    .ListPosts(paging, blogKeys)
                    .ConfigureAwait(false);

            return postHeaders.TryTransformPostHeaders(_transformService);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var postHeaders = await
                _repository
                    .ListPostsByArchive(archiveDate, paging, blogKeys)
                    .ConfigureAwait(false);

            return postHeaders.TryTransformPostHeaders(_transformService);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var postHeaders = await
                _repository
                    .ListPostsByTag(tagName, paging, blogKeys)
                    .ConfigureAwait(false);

            return postHeaders.TryTransformPostHeaders(_transformService);
        }

        public async Task<IReadOnlyList<BlogPostHeader>> SearchPosts(
            string searchText,
            Paging paging = default,
            params BlogKey[] blogKeys)
        {
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var postHeaders = await
                _repository
                    .SearchPosts(searchText, paging, blogKeys)
                    .ConfigureAwait(false);

            return postHeaders.TryTransformPostHeaders(_transformService);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.Queries;
using Microsoft.Extensions.Options;

namespace Blaven.Storage
{
    public abstract class StorageQueryRepositoryBase : IStorageQueryRepository
    {
        private readonly BlogQueryOptions _options;
        private readonly IStorageQueryData _storageQueryData;

        public StorageQueryRepositoryBase(
            IStorageQueryData storageQueryData,
            IOptionsMonitor<BlogQueryOptions> options)
        {
            _storageQueryData = storageQueryData
                ?? throw new ArgumentNullException(nameof(storageQueryData));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _options = options.CurrentValue;
        }

        public Task<BlogMeta?> GetMeta(BlogKey blogKey)
        {
            var meta =
                _storageQueryData
                    .Metas
                    .ApplyOptions(_options)
                    .WhereBlogKey(blogKey)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<BlogPost?> GetPost(string id, BlogKey blogKey)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var post =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKey(blogKey)
                    .OrderByPublishedAt()
                    .FirstOrDefaultById(id);

            return Task.FromResult(post);
        }

        public Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            var post =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKey(blogKey)
                    .OrderByPublishedAt()
                    .FirstOrDefaultBySlug(slug);

            return Task.FromResult(post);
        }

        public Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var dates =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .ToDateList(blogKeys)
                as IReadOnlyList<BlogDateItem>;

            return Task.FromResult(dates);
        }

        public Task<IReadOnlyList<BlogMeta>> ListAllMetas()
        {
            var metas =
                _storageQueryData
                    .Metas
                    .ApplyOptions(_options)
                    .OrderBy(x => x.Name)
                    .ToList()
                as IReadOnlyList<BlogMeta>;

            return Task.FromResult(metas);
        }

        public Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var tags =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .ToTagList(blogKeys)
                as IReadOnlyList<BlogTagItem>;

            return Task.FromResult(tags);
        }

        public Task<IPagedReadOnlyList<BlogPostHeader>> ListPosts(
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKeys(blogKeys)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }

        public Task<IPagedReadOnlyList<BlogPost>> ListPostsFull(
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKeys(blogKeys)
                    .OrderByPublishedAtDescending()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }

        public Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKeys(blogKeys)
                    .WherePublishedAt(archiveDate)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }

        public Task<IPagedReadOnlyList<BlogPostHeader>> ListPostsByTag(
            string tagName,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (tagName is null)
                throw new ArgumentNullException(nameof(tagName));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKeys(blogKeys)
                    .WhereTagName(tagName)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }

        public Task<IReadOnlyList<BlogPostSeriesEpisode>> ListSeriesEpisodes(
            string seriesName,
            IEnumerable<BlogKey> blogKeys)
        {
            if (seriesName is null)
                throw new ArgumentNullException(nameof(seriesName));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var series =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .ToSeriesList(seriesName, blogKeys)
                as IReadOnlyList<BlogPostSeriesEpisode>;

            return Task.FromResult(series);
        }

        public Task<IPagedReadOnlyList<BlogPostHeader>> SearchPosts(
            string searchText,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                _storageQueryData
                    .Posts
                    .ApplyOptions(_options)
                    .WhereBlogKeys(blogKeys)
                    .WhereSearchMatch(searchText)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }
    }
}

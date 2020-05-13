using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Queries;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorageQueryRepository : IStorageQueryRepository
    {
        private readonly IInMemoryStorage _inMemoryStorage;

        public InMemoryStorageQueryRepository(IInMemoryStorage inMemoryStorage)
        {
            _inMemoryStorage = inMemoryStorage
                ?? throw new ArgumentNullException(nameof(inMemoryStorage));
        }

        public Task<BlogMeta?> GetMeta(BlogKey blogKey)
        {
            var meta =
                _inMemoryStorage
                    .Metas
                    .WhereBlogKey(blogKey)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<BlogPost?> GetPost(string id, BlogKey blogKey)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var post =
                _inMemoryStorage
                    .Posts
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
                _inMemoryStorage
                    .Posts
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
                _inMemoryStorage.Posts.ToDateList(blogKeys)
                as IReadOnlyList<BlogDateItem>;

            return Task.FromResult(dates);
        }

        public Task<IReadOnlyList<BlogMeta>> ListAllMetas()
        {
            var metas =
                _inMemoryStorage.Metas.OrderBy(x => x.Name).ToList()
                as IReadOnlyList<BlogMeta>;

            return Task.FromResult(metas);
        }

        public Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var tags =
                _inMemoryStorage.Posts.ToTagList(blogKeys)
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
                _inMemoryStorage
                    .Posts
                    .WhereBlogKeys(blogKeys)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
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
                _inMemoryStorage
                    .Posts
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
                _inMemoryStorage
                    .Posts
                    .WhereBlogKeys(blogKeys)
                    .WhereTagName(tagName)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
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
                _inMemoryStorage
                    .Posts
                    .WhereBlogKeys(blogKeys)
                    .WhereContentContains(searchText)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }
    }
}

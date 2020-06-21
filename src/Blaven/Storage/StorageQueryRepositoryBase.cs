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
        private readonly IQueryable<BlogMeta> _blogMetasQueryable;
        private readonly IQueryable<BlogPost> _blogPostsQueryable;

        public StorageQueryRepositoryBase(
            IQueryable<BlogMeta> blogMetasQueryable,
            IQueryable<BlogPost> blogPostsQueryable,
            IOptionsMonitor<BlogQueryOptions> options)
        {
            if (blogMetasQueryable is null)
                throw new ArgumentNullException(nameof(blogMetasQueryable));
            if (blogPostsQueryable is null)
                throw new ArgumentNullException(nameof(blogPostsQueryable));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            _options = options.CurrentValue;

            _blogMetasQueryable = GetBlogMetasQueryable(blogMetasQueryable);
            _blogPostsQueryable = GetBlogPostsQueryable(blogPostsQueryable);
        }

        private IQueryable<BlogPost> GetBlogPostsQueryable(
            IQueryable<BlogPost> blogPostsQueryable)
        {
            var query = blogPostsQueryable;

            if (!_options.IncludeDraftPosts)
            {
                query = query.Where(x => !x.IsDraft);
            }
            if (!_options.IncludeFuturePosts)
            {
                query = query.Where(x => x.PublishedAt < DateTimeOffset.UtcNow);
            }

            return query;
        }

        private IQueryable<BlogMeta> GetBlogMetasQueryable(
            IQueryable<BlogMeta> blogMetasQueryable)
        {
            return blogMetasQueryable;
        }

        public Task<BlogMeta?> GetMeta(BlogKey blogKey)
        {
            var meta =
                _blogMetasQueryable
                    .WhereBlogKey(blogKey)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<BlogPost?> GetPost(string id, BlogKey blogKey)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var post =
                _blogPostsQueryable
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
                _blogPostsQueryable
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
                _blogPostsQueryable.ToDateList(blogKeys)
                as IReadOnlyList<BlogDateItem>;

            return Task.FromResult(dates);
        }

        public Task<IReadOnlyList<BlogMeta>> ListAllMetas()
        {
            var metas =
                _blogMetasQueryable.OrderBy(x => x.Name).ToList()
                as IReadOnlyList<BlogMeta>;

            return Task.FromResult(metas);
        }

        public Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var tags =
                _blogPostsQueryable.ToTagList(blogKeys)
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
                _blogPostsQueryable
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
                _blogPostsQueryable
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
                _blogPostsQueryable
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
                _blogPostsQueryable
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
                _blogPostsQueryable
                    .WhereBlogKeys(blogKeys)
                    .WhereContentContains(searchText)
                    .OrderByPublishedAtDescending()
                    .OfType<BlogPostHeader>()
                    .ToPagedList(paging);

            return Task.FromResult(posts);
        }
    }
}

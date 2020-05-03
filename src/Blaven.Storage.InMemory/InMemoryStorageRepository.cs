using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Storage.Queries;

namespace Blaven.Storage
{
    public class InMemoryStorageRepository
        : IStorageQueryRepository, IStorageSyncRepository
    {
        private readonly List<BlogMeta> _metas = new List<BlogMeta>();
        private readonly List<BlogPost> _posts = new List<BlogPost>();

        internal IQueryable<BlogMeta> Metas => _metas.AsQueryable();

        internal IQueryable<BlogPost> Posts => _posts.AsQueryable();

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var meta =
                Metas
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAt(lastUpdatedAt)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<BlogMeta?> GetMeta(BlogKey blogKey)
        {
            return GetMeta(blogKey, null);
        }

        public Task<BlogPost?> GetPost(string id, BlogKey blogKey)
        {
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var post = Posts.WhereBlogKey(blogKey).FirstOrDefaultById(id);

            return Task.FromResult(post);
        }

        public Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            var post = Posts.WhereBlogKey(blogKey).FirstOrDefaultBySlug(slug);

            return Task.FromResult(post);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var posts =
                Posts
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(lastUpdatedAt)
                    .OfType<BlogPostBase>()
                    .ToList()
                     as IReadOnlyList<BlogPostBase>;

            return Task.FromResult(posts);
        }

        public Task<IReadOnlyList<BlogDateItem>> ListAllDates(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var dates =
                Posts.ToDateList(blogKeys) as IReadOnlyList<BlogDateItem>;

            return Task.FromResult(dates);
        }

        public Task<IReadOnlyList<BlogMeta>> ListAllMetas()
        {
            var metas = Metas.ToList() as IReadOnlyList<BlogMeta>;

            return Task.FromResult(metas);
        }

        public Task<IReadOnlyList<BlogTagItem>> ListAllTags(
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var tags = Posts.ToTagList(blogKeys) as IReadOnlyList<BlogTagItem>;

            return Task.FromResult(tags);
        }

        public Task<IReadOnlyList<BlogPostHeader>> ListPostHeaders(
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                Posts
                    .WhereBlogKeys(blogKeys)
                    .OfType<BlogPostHeader>()
                    .ApplyPaging(paging)
                    .ToList()
                     as IReadOnlyList<BlogPostHeader>;

            return Task.FromResult(posts);
        }

        public Task<IReadOnlyList<BlogPost>> ListPostsByArchive(
            DateTimeOffset archiveDate,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                Posts
                    .WhereBlogKeys(blogKeys)
                    .WherePublishedAt(archiveDate)
                    .ApplyPaging(paging)
                    .ToList()
                     as IReadOnlyList<BlogPost>;

            return Task.FromResult(posts);
        }

        public Task<IReadOnlyList<BlogPost>> ListPostsByTag(
            string tagName,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (tagName is null)
                throw new ArgumentNullException(nameof(tagName));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                Posts
                    .WhereBlogKeys(blogKeys)
                    .WhereTagName(tagName)
                    .ApplyPaging(paging)
                    .ToList()
                     as IReadOnlyList<BlogPost>;

            return Task.FromResult(posts);
        }

        public Task<IReadOnlyList<BlogPostHeader>> SearchPostHeaders(
            string searchText,
            Paging paging,
            IEnumerable<BlogKey> blogKeys)
        {
            if (searchText is null)
                throw new ArgumentNullException(nameof(searchText));
            if (blogKeys is null)
                throw new ArgumentNullException(nameof(blogKeys));

            var posts =
                Posts
                    .WhereBlogKeys(blogKeys)
                    .WhereContentContains(searchText)
                    .ApplyPaging(paging)
                    .OfType<BlogPostHeader>()
                    .ToList()
                     as IReadOnlyList<BlogPostHeader>;

            return Task.FromResult(posts);
        }

        public Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            IEnumerable<BlogPost> insertedPosts,
            IEnumerable<BlogPost> updatedPosts,
            IEnumerable<BlogPostBase> deletedPosts,
            DateTimeOffset? lastUpdatedAt)
        {
            if (insertedPosts is null)
                throw new ArgumentNullException(nameof(insertedPosts));
            if (updatedPosts is null)
                throw new ArgumentNullException(nameof(updatedPosts));
            if (deletedPosts is null)
                throw new ArgumentNullException(nameof(deletedPosts));

            if (lastUpdatedAt == null)
            {
                _posts.RemoveAll(x => x.BlogKey == blogKey);
            }

            CreateOrUpdateMeta(blogKey, meta);

            foreach (var post in insertedPosts)
            {
                CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in updatedPosts)
            {
                CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in deletedPosts)
            {
                DeletePost(blogKey, post);
            }

            return Task.CompletedTask;
        }

        private void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta)
        {
            if (meta is null)
            {
                return;
            }

            _metas.RemoveAll(x => x.BlogKey == blogKey);

            _metas.Add(meta);
        }

        private void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            DeletePost(blogKey, post);

            _posts.Add(post);
        }

        private void DeletePost(BlogKey blogKey, BlogPostBase post)
        {
            _posts.RemoveAll(x => x.BlogKey == blogKey && x.Id == post.Id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public class InMemoryStorageRepository
        : IStorageSyncRepository, IStorageQueryRepository
    {
        internal List<BlogMeta> Metas { get; } = new List<BlogMeta>();
        internal List<BlogPost> Posts { get; } = new List<BlogPost>();

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var meta =
                Metas
                    .Where(x => x.BlogKey == blogKey || !blogKey.HasValue)
                    .Where(x => x.UpdatedAt > lastUpdatedAt
                        || lastUpdatedAt == null)
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

            var post =
                Posts
                    .Where(x => x.BlogKey == blogKey || !blogKey.HasValue)
                    .Where(x => x.Id == id)
                    .FirstOrDefault();

            return Task.FromResult<BlogPost?>(post);
        }

        public Task<BlogPost?> GetPostBySlug(string slug, BlogKey blogKey)
        {
            if (slug is null)
                throw new ArgumentNullException(nameof(slug));

            var post =
                Posts
                    .Where(x => x.BlogKey == blogKey || !blogKey.HasValue)
                    .Where(x => x.Slug == slug)
                    .FirstOrDefault();

            return Task.FromResult<BlogPost?>(post);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var posts =
                Posts
                    .Where(x => x.BlogKey == blogKey || !blogKey.HasValue)
                    .Where(x => x.UpdatedAt > lastUpdatedAt
                        || lastUpdatedAt == null)
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
                (from post in Posts
                 where !blogKeys.Any() || blogKeys.Contains(post.BlogKey)
                 let publishedAt = post.PublishedAt
                 where publishedAt != null
                 let date =
                    new DateTime(publishedAt.Value.Year, publishedAt.Value.Month, 1)
                 group post by new { post.BlogKey, Date = date } into g
                 orderby g.Key.Date descending
                 select new BlogDateItem
                 {
                     BlogKey = g.Key.BlogKey,
                     Count = g.Count(),
                     Date = g.Key.Date
                 }).ToList()
                 as IReadOnlyList<BlogDateItem>;

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

            var tags =
                (from post in Posts
                 where !blogKeys.Any() || blogKeys.Contains(post.BlogKey)
                 from tag in post.Tags
                 group post by new { post.BlogKey, Tag = tag } into g
                 orderby g.Key.Tag ascending
                 select new BlogTagItem
                 {
                     BlogKey = g.Key.BlogKey,
                     Count = g.Count(),
                     Name = g.Key.Tag
                 }).ToList()
                 as IReadOnlyList<BlogTagItem>;

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
                    .Where(x => !blogKeys.Any() || blogKeys.Contains(x.BlogKey))
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
                    .Where(x => !blogKeys.Any() || blogKeys.Contains(x.BlogKey))
                    .Where(x =>
                        x.PublishedAt?.Year == archiveDate.Year
                        && x.PublishedAt?.Month == archiveDate.Month)
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
                    .Where(x => !blogKeys.Any() || blogKeys.Contains(x.BlogKey))
                    .Where(x =>
                        x.Tags.Contains(
                            tagName,
                            StringComparer.InvariantCultureIgnoreCase))
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
                    .Where(x => !blogKeys.Any() || blogKeys.Contains(x.BlogKey))
                    .Where(x =>
                        x.Content.ContainsIgnoreCase(searchText))
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
                Posts.RemoveAll(x => x.BlogKey == blogKey);
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

            Metas.RemoveAll(x => x.BlogKey == blogKey);

            Metas.Add(meta);
        }

        private void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            DeletePost(blogKey, post);

            Posts.Add(post);
        }

        private void DeletePost(BlogKey blogKey, BlogPostBase post)
        {
            Posts.RemoveAll(x => x.BlogKey == blogKey && x.Id == post.Id);
        }
    }
}

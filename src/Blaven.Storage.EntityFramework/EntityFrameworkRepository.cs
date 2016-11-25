using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework
{
    public class EntityFrameworkRepository : IRepository
    {
        public EntityFrameworkRepository(BlavenDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.DbContext = dbContext;
        }

        internal BlavenDbContext DbContext { get; }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            var metas = this.DbContext.BlogMetas.OrderBy(x => x.Name);
            return metas;
        }

        public async Task<BlogMeta> GetBlogMeta(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var meta =
                await
                    this.DbContext.BlogMetas.SingleOrDefaultAsync(
                        x => x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase));
            return meta;
        }

        public async Task<BlogPost> GetPost(string blogKey, string blavenId)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (blavenId == null)
            {
                throw new ArgumentNullException(nameof(blavenId));
            }

            var post =
                await
                    this.DbContext.BlogPosts.OrderBy(x => x.PublishedAt)
                        .SingleOrDefaultAsync(
                            x =>
                                x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase)
                                && x.BlavenId.Equals(blavenId, StringComparison.OrdinalIgnoreCase));
            return post;
        }

        public async Task<BlogPost> GetPostBySourceId(string blogKey, string sourceId)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            var post =
                await
                    this.DbContext.BlogPosts.OrderBy(x => x.PublishedAt)
                        .SingleOrDefaultAsync(
                            x => x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase) && x.SourceId == sourceId);
            return post;
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var archive = from post in this.DbContext.BlogPosts
                          where blogKeyList.Contains(post.BlogKey.ToLowerInvariant())
                          let date = new DateTime(post.PublishedAt.Value.Year, post.PublishedAt.Value.Month, 1)
                          group post by new { post.BlogKey, Date = date }
                          into g
                          orderby g.Key.Date descending
                          select new BlogArchiveItem { BlogKey = g.Key.BlogKey, Date = g.Key.Date, Count = g.Count() };
            return archive;
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var tags = from post in this.DbContext.BlogPosts
                       where blogKeyList.Contains(post.BlogKey.ToLowerInvariant())
                       from tag in post.BlogPostTags
                       group tag by new { post.BlogKey, Name = tag.Text.ToLowerInvariant() }
                       into g
                       select
                       new BlogTagItem
                           {
                               BlogKey = g.Key.BlogKey,
                               Name = g.Select(x => x.Text).First(),
                               Count = g.Count()
                           };
            return tags;
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var heads =
                this.DbContext.BlogPosts.Where(x => blogKeyList.Contains(x.BlogKey.ToLowerInvariant()))
                    .OfType<BlogPostHead>()
                    .OrderByDescending(x => x.PublishedAt);
            return heads;
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var posts =
                this.DbContext.BlogPosts.Where(x => blogKeyList.Contains(x.BlogKey))
                    .OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var archiveDateStart = new DateTime(archiveDate.Year, archiveDate.Month, 1);
            var archiveDateEnd = archiveDateStart.AddMonths(1);

            var posts =
                this.DbContext.BlogPosts.Where(
                    x =>
                        blogKeyList.Contains(x.BlogKey.ToLowerInvariant()) && x.PublishedAt >= archiveDateStart
                        && x.PublishedAt < archiveDateEnd).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            var posts =
                this.DbContext.BlogPosts.Where(
                        x =>
                            blogKeyList.Contains(x.BlogKey.ToLowerInvariant())
                            && x.BlogPostTags.Any(t => t.Text.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
                    .OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> SearchPosts(IEnumerable<string> blogKeys, string search)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }
            if (search == null)
            {
                throw new ArgumentNullException(nameof(search));
            }

            var blogKeyList = GetBlogKeys(blogKeys);

            string searchText = search.ToLowerInvariant();

            var posts =
                this.DbContext.BlogPosts.Where(
                        x =>
                            blogKeyList.Contains(x.BlogKey.ToLowerInvariant())
                            && ((x.Content != null && x.Content.ToLowerInvariant().Contains(searchText))
                                || (x.Summary != null && x.Summary.ToLowerInvariant().Contains(searchText))
                                || (x.Title != null && x.Title.ToLowerInvariant().Contains(searchText))
                                || (x.BlogAuthor != null && x.BlogAuthor.Name != null
                                    && x.BlogAuthor.Name.ToLowerInvariant().Contains(searchText))
                                || x.BlogPostTags.Any(t => t.Text.Equals(searchText, StringComparison.OrdinalIgnoreCase))))
                    .OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        private static IList<string> GetBlogKeys(IEnumerable<string> blogKeys)
        {
            return blogKeys.Select(x => x.ToLowerInvariant()).ToList();
        }
    }
}
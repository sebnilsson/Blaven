using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.Data.Tests
{
    public class FakeRepository : IRepository
    {
        private readonly ICollection<BlogPost> blogPosts;

        private readonly ICollection<BlogMeta> blogMetas;

        public FakeRepository(IEnumerable<BlogPost> blogPosts = null, IEnumerable<BlogMeta> blogMetas = null)
        {
            this.blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
            this.blogMetas = (blogMetas ?? Enumerable.Empty<BlogMeta>()).ToList();
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            return this.blogMetas.AsQueryable();
        }

        public Task<BlogMeta> GetBlogMeta(string blogKey)
        {
            var meta = this.blogMetas.FirstOrDefault(x => x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(meta);
        }

        public Task<BlogPost> GetPost(string blogKey, string blavenId)
        {
            var post =
                this.blogPosts.FirstOrDefault(
                    x =>
                        x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase)
                        && x.BlavenId.Equals(blavenId, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(post);
        }

        public Task<BlogPost> GetPostBySourceId(string blogKey, string sourceId)
        {
            var post =
                this.blogPosts.FirstOrDefault(
                    x =>
                        x.BlogKey.Equals(blogKey, StringComparison.OrdinalIgnoreCase)
                        && x.SourceId.Equals(sourceId, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(post);
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            var archive = from post in this.blogPosts
                          let publishedAt = post.PublishedAt
                          where blogKeys.Contains(post.BlogKey)
                          group post by new { publishedAt.Value.Year, publishedAt.Value.Month }
                          into g
                          select
                          new BlogArchiveItem { Date = new DateTime(g.Key.Year, g.Key.Month, 1), Count = g.Count() };
            return archive.AsQueryable();
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            var tags = from post in this.blogPosts
                       where blogKeys.Contains(post.BlogKey) && post.PublishedAt != null && post.BlogPostTags != null
                       from tag in post.BlogPostTags
                       group tag by tag.Text
                       into g
                       select new BlogTagItem { Name = g.Key, Count = g.Count() };
            return tags.AsQueryable();
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            var postHeads = this.blogPosts.Where(x => blogKeys.Contains(x.BlogKey));
            return postHeads.AsQueryable();
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            var posts = this.blogPosts.Where(x => blogKeys.Contains(x.BlogKey));
            return posts.AsQueryable();
        }

        public IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate)
        {
            var posts =
                this.blogPosts.Where(
                    x =>
                        blogKeys.Contains(x.BlogKey) && x.PublishedAt != null
                        && x.PublishedAt.Value.Year == archiveDate.Year
                        && x.PublishedAt.Value.Month == archiveDate.Month);
            return posts.AsQueryable();
        }

        public IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName)
        {
            var posts =
                this.blogPosts.Where(
                    x =>
                        blogKeys.Contains(x.BlogKey) && x.BlogPostTags != null
                        && x.TagTexts.Contains(tagName, StringComparer.InvariantCultureIgnoreCase));
            return posts.AsQueryable();
        }

        public IQueryable<BlogPost> SearchPosts(IEnumerable<string> blogKeys, string search)
        {
            var posts =
                this.blogPosts.Where(
                    x =>
                        blogKeys.Contains(x.BlogKey) && StringContainsIgnoreCase(x.Title, search)
                        || StringContainsIgnoreCase(x.Summary, search) || StringContainsIgnoreCase(x.Content, search));
            return posts.AsQueryable();
        }

        private static bool StringContainsIgnoreCase(string source, string value)
        {
            if (source == null || value == null)
            {
                return false;
            }

            int indexOf = source.IndexOf(value, StringComparison.OrdinalIgnoreCase);

            bool contains = (indexOf >= 0);
            return contains;
        }
    }
}
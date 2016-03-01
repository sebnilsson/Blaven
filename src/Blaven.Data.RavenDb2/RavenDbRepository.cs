using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Data.RavenDb2.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.Data.RavenDb2
{
    public class RavenDbRepository : IRepository
    {
        public RavenDbRepository(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            this.DocumentStore = documentStore;
        }

        public IDocumentStore DocumentStore { get; }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            using (var session = this.DocumentStore.OpenSession())
            {
                var metas = session.Query<BlogMeta>();
                return metas;
            }
        }

        public BlogMeta GetBlogMeta(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            string blogMetaId = RavenDbIdConventions.GetBlogMetaId(blogKey);

            using (var session = this.DocumentStore.OpenSession())
            {
                var meta = session.Load<BlogMeta>(blogMetaId);
                return meta;
            }
        }

        public BlogPost GetPost(string blogKey, string blavenId)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (blavenId == null)
            {
                throw new ArgumentNullException(nameof(blavenId));
            }

            string blogPostId = RavenDbIdConventions.GetBlogPostId(blogKey, blavenId);

            using (var session = this.DocumentStore.OpenSession())
            {
                var post = session.Load<BlogPost>(blogPostId);
                return post;
            }
        }

        public BlogPost GetPostBySourceId(string blogKey, string sourceId)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var post =
                    session.Query<BlogPost, BlogPostsIndex>()
                        .OrderBy(x => x.PublishedAt)
                        .FirstOrDefault(x => x.BlogKey == blogKey && x.SourceId == sourceId);
                return post;
            }
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var archive = session.Query<BlogArchiveItem, ArchiveCountIndex>().Where(x => x.BlogKey.In(blogKeys));
                return archive;
            }
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var tags = session.Query<BlogTagItem, TagsCountIndex>().Where(x => x.BlogKey.In(blogKeys));
                return tags;
            }
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var heads = session.Query<BlogPostHead, BlogPostsIndex>().Where(x => x.BlogKey.In(blogKeys));
                return heads;
            }
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var posts = session.Query<BlogPost, BlogPostsIndex>().Where(x => x.BlogKey.In(blogKeys));
                return posts;
            }
        }

        public IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime archiveDate)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            var archiveDateStart = new DateTime(archiveDate.Year, archiveDate.Month, 1);
            var archiveDateEnd = archiveDateStart.AddMonths(1);

            using (var session = this.DocumentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPost, BlogPostsIndex>()
                        .Where(
                            x =>
                            x.BlogKey.In(blogKeys) && x.PublishedAt >= archiveDateStart
                            && x.PublishedAt < archiveDateEnd);
                return posts;
            }
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

            using (var session = this.DocumentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPost, BlogPostsIndex>()
                        .Where(x => x.BlogKey.In(blogKeys) && x.Tags.Any(tag => tag == tagName));
                return posts;
            }
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

            string escapedSearch = search.Replace("\"", "\\\"");

            var escapedBlogKeys = blogKeys.Select(key => key.Replace("\"", "\\\""));

            string blogKeysValues = string.Join(" OR ", escapedBlogKeys.Select(key => $"BlogKey:\"{key}\""));

            string blogKeysClause = !string.IsNullOrWhiteSpace(blogKeysValues) ? $" AND ({blogKeysValues})" : null;

            string whereClause = $"Content:\"{escapedSearch}\" {blogKeysClause}";

            using (var session = this.DocumentStore.OpenSession())
            {
                var posts =
                    session.Advanced.LuceneQuery<BlogPost, SearchBlogPostsIndex>().Where(whereClause).AsQueryable();
                return posts;
            }
        }
    }
}
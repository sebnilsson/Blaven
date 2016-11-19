using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Data.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.Data.RavenDb
{
    public class RavenDbRepository : IRepository
    {
        private readonly IDocumentStore documentStore;

        public RavenDbRepository(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            this.documentStore = documentStore;
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            using (var session = this.documentStore.OpenSession())
            {
                var metas = session.Query<BlogMeta>().OrderBy(x => x.Name);
                return metas;
            }
        }

        public async Task<BlogMeta> GetBlogMeta(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            string blogMetaId = RavenDbIdConventions.GetBlogMetaId(blogKey);

            using (var session = this.documentStore.OpenAsyncSession())
            {
                var meta = await session.LoadAsync<BlogMeta>(blogMetaId);
                return meta;
            }
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

            string blogPostId = RavenDbIdConventions.GetBlogPostId(blogKey, blavenId);

            using (var session = this.documentStore.OpenAsyncSession())
            {
                var post = await session.LoadAsync<BlogPost>(blogPostId);
                return post;
            }
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

            using (var session = this.documentStore.OpenAsyncSession())
            {
                var post =
                    await
                        session.Query<BlogPost, BlogPostsIndex>()
                            .OrderBy(x => x.PublishedAt)
                            .FirstOrDefaultAsync(x => x.BlogKey == blogKey && x.SourceId == sourceId);
                return post;
            }
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.documentStore.OpenSession())
            {
                var archive =
                    session.Query<BlogArchiveItem, ArchiveCountIndex>()
                        .Where(x => x.BlogKey.In(blogKeys))
                        .OrderByDescending(x => x.Date);
                return archive;
            }
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.documentStore.OpenSession())
            {
                var tags =
                    session.Query<BlogTagItem, TagsCountIndex>().Where(x => x.BlogKey.In(blogKeys)).OrderBy(x => x.Name);
                return tags;
            }
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.documentStore.OpenSession())
            {
                var heads =
                    session.Query<BlogPostHead, BlogPostsIndex>()
                        .Where(x => x.BlogKey.In(blogKeys))
                        .OrderByDescending(x => x.PublishedAt);
                return heads;
            }
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException(nameof(blogKeys));
            }

            using (var session = this.documentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPost, BlogPostsIndex>()
                        .Where(x => x.BlogKey.In(blogKeys))
                        .OrderByDescending(x => x.PublishedAt);
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

            using (var session = this.documentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPost, BlogPostsIndex>()
                        .Where(
                            x =>
                                x.BlogKey.In(blogKeys) && x.PublishedAt >= archiveDateStart
                                && x.PublishedAt < archiveDateEnd)
                        .OrderByDescending(x => x.PublishedAt);
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

            using (var session = this.documentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPostTagsIndex.Result, BlogPostTagsIndex>()
                        .Where(x => x.BlogKey.In(blogKeys) && x.TagText == tagName)
                        .OfType<BlogPost>()
                        .OrderByDescending(x => x.PublishedAt);
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

            using (var session = this.documentStore.OpenSession())
            {
                var posts = session.Advanced
#if RAVENDB2
                    .LuceneQuery<BlogPost, SearchBlogPostsIndex>()
#else
                    .DocumentQuery<BlogPost, SearchBlogPostsIndex>()
#endif
                    .Where(whereClause).AsQueryable();

                return posts;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataStorage.RavenDb.Indexes;
using Blaven.Synchronization;
using Raven.Client;

namespace Blaven.DataStorage.RavenDb
{
    public class RavenDbDataStorage : IDataStorage
    {
        public RavenDbDataStorage(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            this.DocumentStore = documentStore;
        }

        internal IDocumentStore DocumentStore { get; }

        public async Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            using (var session = this.DocumentStore.OpenAsyncSession())
            {
                var lastPost =
                    await
                        session.Query<BlogPostHead, BlogPostsIndex>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                            .Where(x => x.BlogKey == blogSetting.BlogKey)
                            .OrderByDescending(x => x.UpdatedAt)
                            .FirstOrDefaultAsync();

                return lastPost?.UpdatedAt;
            }
        }

        public async Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(
            BlogSetting blogSetting,
            DateTime? lastUpdatedAt = null)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            using (var session = this.DocumentStore.OpenAsyncSession())
            {
                var posts =
                    await
                        session.Query<BlogPostHead, BlogPostsIndex>()
                            .Customize(x => x.WaitForNonStaleResultsAsOfLastWrite())
                            .Where(x => x.BlogKey == blogSetting.BlogKey && x.UpdatedAt > lastUpdatedAt)
                            .OrderByDescending(x => x.PublishedAt)
                            .ProjectFromIndexFieldsInto<BlogPostBase>()
                            .ToListAllAsync();

                return posts.ToReadOnlyList();
            }
        }

        public async Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (blogMeta == null)
            {
                throw new ArgumentNullException(nameof(blogMeta));
            }

            using (var session = this.DocumentStore.OpenAsyncSession())
            {
                string blogMetaId = RavenDbIdConventions.GetBlogMetaId(blogSetting.BlogKey);

                var existingMeta = await session.LoadAsync<BlogMeta>(blogMetaId);
                if (existingMeta == null)
                {
                    existingMeta = new BlogMeta { BlogKey = blogSetting.BlogKey };
                    await session.StoreAsync(existingMeta);
                }

                existingMeta.Description = blogMeta.Description;
                existingMeta.Name = blogMeta.Name;
                existingMeta.PublishedAt = blogMeta.PublishedAt;
                existingMeta.SourceId = blogMeta.SourceId;
                existingMeta.UpdatedAt = blogMeta.UpdatedAt;
                existingMeta.Url = blogMeta.Url;

                await session.SaveChangesAsync();
            }
        }

        public async Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (changeSet == null)
            {
                throw new ArgumentNullException(nameof(changeSet));
            }

            using (var session = this.DocumentStore.OpenAsyncSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

                await DeletedPosts(session, changeSet.DeletedBlogPosts);
                await InsertOrUpdatePosts(session, changeSet.InsertedBlogPosts);
                await InsertOrUpdatePosts(session, changeSet.UpdatedBlogPosts);

                await session.SaveChangesAsync();
            }
        }

        private static async Task DeletedPosts(IAsyncDocumentSession session, IEnumerable<BlogPostBase> deletedPosts)
        {
            foreach (var deletedPost in deletedPosts)
            {
                string postId = RavenDbIdConventions.GetBlogPostId(deletedPost.BlogKey, deletedPost.BlavenId);

                var existingPost = await session.LoadAsync<BlogPost>(postId);
                if (existingPost != null)
                {
                    session.Delete(existingPost);
                }
            }
        }

        private static async Task InsertOrUpdatePosts(
            IAsyncDocumentSession session,
            IEnumerable<BlogPost> insertedOrUpdatedPosts)
        {
            foreach (var post in insertedOrUpdatedPosts)
            {
                string postId = RavenDbIdConventions.GetBlogPostId(post.BlogKey, post.BlavenId);

                var existingPost = await session.LoadAsync<BlogPost>(postId);
                if (existingPost == null)
                {
                    existingPost = new BlogPost { BlogKey = post.BlogKey, BlavenId = post.BlavenId };
                    await session.StoreAsync(existingPost);
                }

                existingPost.BlogAuthor = post.BlogAuthor;
                existingPost.Content = post.Content;
                existingPost.Hash = post.Hash;
                existingPost.ImageUrl = post.ImageUrl;
                existingPost.PublishedAt = post.PublishedAt;
                existingPost.SourceId = post.SourceId;
                existingPost.SourceUrl = post.SourceUrl;
                existingPost.Summary = post.Summary;
                existingPost.BlogPostTags = post.BlogPostTags;
                existingPost.Title = post.Title;
                existingPost.UpdatedAt = post.UpdatedAt;
                existingPost.UrlSlug = post.UrlSlug;
            }
        }
    }
}
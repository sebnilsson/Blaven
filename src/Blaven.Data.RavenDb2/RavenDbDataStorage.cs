using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Data.RavenDb2.Indexes;
using Blaven.Synchronization;
using Raven.Client;

namespace Blaven.Data.RavenDb2
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

        public IDocumentStore DocumentStore { get; }

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

        public async Task<IReadOnlyList<BlogPostBase>> GetPostBases(BlogSetting blogSetting, DateTime? lastUpdatedAt = null)
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
                            .AsProjection<BlogPostBase>()
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

                var updatedBlogMeta = await session.LoadAsync<BlogMeta>(blogMetaId);
                if (updatedBlogMeta == null)
                {
                    updatedBlogMeta = new BlogMeta { BlogKey = blogSetting.BlogKey };
                    await session.StoreAsync(updatedBlogMeta);
                }

                updatedBlogMeta.Description = blogMeta.Description;
                updatedBlogMeta.Name = blogMeta.Name;
                updatedBlogMeta.PublishedAt = blogMeta.PublishedAt;
                updatedBlogMeta.SourceId = blogMeta.SourceId;
                updatedBlogMeta.UpdatedAt = blogMeta.UpdatedAt;
                updatedBlogMeta.Url = blogMeta.Url;

                await session.SaveChangesAsync();
            }
        }

        public async Task SaveChanges(BlogSetting blogSetting, BlogSyncChangeSet changeSet)
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

        private static async Task InsertOrUpdatePosts(IAsyncDocumentSession session, IEnumerable<BlogPost> insertedOrUpdatedPosts)
        {
            foreach (var post in insertedOrUpdatedPosts)
            {
                string postId = RavenDbIdConventions.GetBlogPostId(post.BlogKey, post.BlavenId);

                var ravenDbPost = await session.LoadAsync<BlogPost>(postId);
                if (ravenDbPost == null)
                {
                    ravenDbPost = new BlogPost { BlogKey = post.BlogKey, BlavenId = post.BlavenId };
                    await session.StoreAsync(ravenDbPost);
                }

                ravenDbPost.Author = post.Author;
                ravenDbPost.Content = post.Content;
                ravenDbPost.Hash = post.Hash;
                ravenDbPost.ImageUrl = post.ImageUrl;
                ravenDbPost.PublishedAt = post.PublishedAt;
                ravenDbPost.SourceId = post.SourceId;
                ravenDbPost.SourceUrl = post.SourceUrl;
                ravenDbPost.Summary = post.Summary;
                ravenDbPost.Tags = post.Tags;
                ravenDbPost.Title = post.Title;
                ravenDbPost.UpdatedAt = post.UpdatedAt;
                ravenDbPost.UrlSlug = post.UrlSlug;
            }
        }
    }
}
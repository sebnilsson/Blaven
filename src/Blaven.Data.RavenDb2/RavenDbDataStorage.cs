using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.BlogSources;
using Blaven.Data.RavenDb2.Indexes;
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

        public IReadOnlyCollection<BlogPostBase> GetPostBases(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                var posts =
                    session.Query<BlogPostBase, BlogPostsIndex>()
                        .Customize(x => x.WaitForNonStaleResultsAsOfNow())
                        .Where(x => x.BlogKey == blogSetting.BlogKey)
                        .AsProjection<BlogPostBase>()
                        .ToListAll();

                return posts.ToReadOnlyList();
            }
        }

        public void SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (blogMeta == null)
            {
                throw new ArgumentNullException(nameof(blogMeta));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                string blogMetaId = RavenDbIdConventions.GetBlogMetaId(blogSetting.BlogKey);

                var updatedBlogMeta = session.Load<BlogMeta>(blogMetaId);
                if (updatedBlogMeta == null)
                {
                    updatedBlogMeta = new BlogMeta { BlogKey = blogSetting.BlogKey };
                    session.Store(updatedBlogMeta);
                }

                updatedBlogMeta.Description = blogMeta.Description;
                updatedBlogMeta.Name = blogMeta.Name;
                updatedBlogMeta.PublishedAt = blogMeta.PublishedAt;
                updatedBlogMeta.SourceId = blogMeta.SourceId;
                updatedBlogMeta.UpdatedAt = blogMeta.UpdatedAt;
                updatedBlogMeta.Url = blogMeta.Url;

                session.SaveChanges();
            }
        }

        public void SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (changeSet == null)
            {
                throw new ArgumentNullException(nameof(changeSet));
            }

            using (var session = this.DocumentStore.OpenSession())
            {
                session.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

                DeletedPosts(session, changeSet.DeletedBlogPosts);
                InsertOrUpdatePosts(session, changeSet.InsertedBlogPosts);
                InsertOrUpdatePosts(session, changeSet.UpdatedBlogPosts);

                session.SaveChanges();
            }
        }

        private static void DeletedPosts(IDocumentSession session, IEnumerable<BlogPostBase> deletedPosts)
        {
            foreach (var deletedPost in deletedPosts)
            {
                string postId = RavenDbIdConventions.GetBlogPostId(deletedPost.BlogKey, deletedPost.BlavenId);

                var existingPost = session.Load<BlogPost>(postId);
                if (existingPost != null)
                {
                    session.Delete(existingPost);
                }
            }
        }

        private static void InsertOrUpdatePosts(IDocumentSession session, IEnumerable<BlogPost> insertedOrUpdatedPosts)
        {
            foreach (var post in insertedOrUpdatedPosts)
            {
                string postId = RavenDbIdConventions.GetBlogPostId(post.BlogKey, post.BlavenId);

                var ravenDbPost = session.Load<BlogPost>(postId);
                if (ravenDbPost == null)
                {
                    ravenDbPost = new BlogPost { BlogKey = post.BlogKey, BlavenId = post.BlavenId };
                    session.Store(ravenDbPost);
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
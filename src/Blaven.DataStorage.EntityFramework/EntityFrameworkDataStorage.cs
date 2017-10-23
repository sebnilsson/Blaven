using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Synchronization;
using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework
{
    public class EntityFrameworkDataStorage : IDataStorage
    {
        public EntityFrameworkDataStorage(BlavenDbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        internal BlavenDbContext DbContext { get; }

        public async Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));

            var posts = await DbContext.BlogPosts
                            .Where(
                                x => x.BlogKey.Equals(blogSetting.BlogKey, StringComparison.OrdinalIgnoreCase)
                                     && (lastUpdatedAt == null || x.UpdatedAt > lastUpdatedAt))
                            .OrderByDescending(x => x.PublishedAt)
                            .ToListAsync();

            return posts.ToReadOnlyList();
        }

        public async Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));

            var lastPost = await DbContext.BlogPosts.OrderByDescending(x => x.UpdatedAt)
                               .FirstOrDefaultAsync(
                                   x => x.BlogKey.Equals(blogSetting.BlogKey, StringComparison.OrdinalIgnoreCase));

            return lastPost?.UpdatedAt;
        }

        public async Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));
            if (blogMeta == null)
                throw new ArgumentNullException(nameof(blogMeta));

            var existingMeta = await DbContext.BlogMetas.SingleOrDefaultLocalOrSourceAsync(
                                   x => x.BlogKey.Equals(blogSetting.BlogKey, StringComparison.OrdinalIgnoreCase));

            if (existingMeta == null)
            {
                existingMeta = new BlogMeta
                               {
                                   BlogKey = blogSetting.BlogKey
                               };
                await DbContext.BlogMetas.AddAsync(existingMeta);
            }

            existingMeta.Description = blogMeta.Description;
            existingMeta.Name = blogMeta.Name;
            existingMeta.PublishedAt = blogMeta.PublishedAt;
            existingMeta.SourceId = blogMeta.SourceId;
            existingMeta.UpdatedAt = blogMeta.UpdatedAt;
            existingMeta.Url = blogMeta.Url;

            await DbContext.SaveChangesAsync();
        }

        public async Task SaveChanges(BlogSetting blogSetting, BlogSyncPostsChangeSet changeSet)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));
            if (changeSet == null)
                throw new ArgumentNullException(nameof(changeSet));

            await DeletedPosts(changeSet.DeletedBlogPosts);
            await InsertOrUpdatePosts(changeSet.InsertedBlogPosts);
            await InsertOrUpdatePosts(changeSet.UpdatedBlogPosts);

            await DbContext.SaveChangesAsync();
        }

        private async Task DeletedPosts(IEnumerable<BlogPostBase> deletedPosts)
        {
            foreach (var deletedPost in deletedPosts)
            {
                var existingPost = await DbContext.BlogPosts.SingleOrDefaultLocalOrSourceAsync(
                                       x => x.BlogKey.Equals(deletedPost.BlogKey, StringComparison.OrdinalIgnoreCase)
                                            && x.BlavenId.Equals(
                                                deletedPost.BlavenId,
                                                StringComparison.OrdinalIgnoreCase));

                if (existingPost != null)
                    DbContext.BlogPosts.Remove(existingPost);
            }
        }

        private async Task InsertOrUpdatePosts(IEnumerable<BlogPost> insertedOrUpdatedPosts)
        {
            foreach (var post in insertedOrUpdatedPosts)
            {
                var existingPost = await DbContext.BlogPosts.SingleOrDefaultLocalOrSourceAsync(
                                       x => x.BlogKey.Equals(post.BlogKey, StringComparison.OrdinalIgnoreCase)
                                            && x.BlavenId.Equals(post.BlavenId, StringComparison.OrdinalIgnoreCase));

                if (existingPost == null)
                {
                    existingPost = new BlogPost
                                   {
                                       BlogKey = post.BlogKey,
                                       BlavenId = post.BlavenId
                                   };
                    await DbContext.BlogPosts.AddAsync(existingPost);
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
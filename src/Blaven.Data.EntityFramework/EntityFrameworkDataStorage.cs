using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Synchronization;

using Microsoft.EntityFrameworkCore;

namespace Blaven.Data.EntityFramework
{
    public class EntityFrameworkDataStorage : IDataStorage
    {
        private readonly BlavenDbContext dbContext;

        public EntityFrameworkDataStorage(BlavenDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this.dbContext = dbContext;
        }

        public async Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var lastPost =
                await
                    this.dbContext.BlogPosts.Where(x => x.BlogKey == blogSetting.BlogKey)
                        .OrderByDescending(x => x.UpdatedAt)
                        .FirstOrDefaultAsync();

            return lastPost?.UpdatedAt;

        }

        public async Task<IReadOnlyList<BlogPostBase>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var posts =
                await
                    this.dbContext.BlogPosts.Where(x => x.BlogKey == blogSetting.BlogKey && x.UpdatedAt > lastUpdatedAt)
                        .OrderByDescending(x => x.PublishedAt)
                        .ToListAsync();

            return posts.ToReadOnlyList();
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

            var existingMeta =
                await this.dbContext.BlogMetas.SingleOrDefaultAsync(x => x.BlogKey == blogSetting.BlogKey);
            if (existingMeta == null)
            {
                existingMeta = new BlogMeta { BlogKey = blogSetting.BlogKey };
                this.dbContext.BlogMetas.Add(existingMeta);
            }

            existingMeta.Description = blogMeta.Description;
            existingMeta.Name = blogMeta.Name;
            existingMeta.PublishedAt = blogMeta.PublishedAt;
            existingMeta.SourceId = blogMeta.SourceId;
            existingMeta.UpdatedAt = blogMeta.UpdatedAt;
            existingMeta.Url = blogMeta.Url;

            await this.dbContext.SaveChangesAsync();
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

            await this.DeletedPosts(changeSet.DeletedBlogPosts);
            await this.InsertOrUpdatePosts(changeSet.InsertedBlogPosts);
            await this.InsertOrUpdatePosts(changeSet.UpdatedBlogPosts);

            await this.dbContext.SaveChangesAsync();
        }

        private async Task DeletedPosts(IEnumerable<BlogPostBase> deletedPosts)
        {
            foreach (var deletedPost in deletedPosts)
            {
                var existingPost =
                    await
                        this.dbContext.BlogPosts.SingleOrDefaultAsync(
                            x => x.BlogKey == deletedPost.BlogKey && x.BlavenId == deletedPost.BlavenId);
                if (existingPost != null)
                {
                    this.dbContext.BlogPosts.Remove(existingPost);
                }
            }
        }

        private async Task InsertOrUpdatePosts(IEnumerable<BlogPost> insertedOrUpdatedPosts)
        {
            foreach (var post in insertedOrUpdatedPosts)
            {
                var existingPost =
                    await
                        this.dbContext.BlogPosts.SingleOrDefaultAsync(
                            x => x.BlogKey == post.BlogKey && x.BlavenId == post.BlavenId);
                if (existingPost == null)
                {
                    existingPost = new BlogPost { BlogKey = post.BlogKey, BlavenId = post.BlavenId };
                    this.dbContext.BlogPosts.Add(existingPost);
                }

                existingPost.Author = post.Author;
                existingPost.Content = post.Content;
                existingPost.Hash = post.Hash;
                existingPost.ImageUrl = post.ImageUrl;
                existingPost.PublishedAt = post.PublishedAt;
                existingPost.SourceId = post.SourceId;
                existingPost.SourceUrl = post.SourceUrl;
                existingPost.Summary = post.Summary;
                existingPost.Tags = post.Tags;
                existingPost.Title = post.Title;
                existingPost.UpdatedAt = post.UpdatedAt;
                existingPost.UrlSlug = post.UrlSlug;
            }
        }
    }
}
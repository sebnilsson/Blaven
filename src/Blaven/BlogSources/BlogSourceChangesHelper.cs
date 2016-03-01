using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources
{
    internal static class BlogSourceChangesHelper
    {
        public static BlogSourceChangeSet GetChangeSet(
            string blogKey,
            ICollection<BlogPost> sourcePosts,
            ICollection<BlogPostBase> dbPosts,
            DateTime? lastUpdatedAt)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (sourcePosts == null)
            {
                throw new ArgumentNullException(nameof(sourcePosts));
            }
            if (dbPosts == null)
            {
                throw new ArgumentNullException(nameof(dbPosts));
            }

            var changeSet = new BlogSourceChangeSet(blogKey);

            if (lastUpdatedAt == null || lastUpdatedAt == DateTime.MinValue)
            {
                SyncDeletedPosts(sourcePosts, dbPosts, changeSet);
            }

            SyncInsertedPosts(sourcePosts, dbPosts, changeSet);

            SyncModifiedPosts(sourcePosts, dbPosts, changeSet, lastUpdatedAt);

            return changeSet;
        }

        private static void SyncDeletedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet)
        {
            var sourceIds = sourcePosts.Select(x => x.SourceId).ToList();

            var deletedPosts = dbPosts.Where(post => !sourceIds.Contains(post.SourceId)).ToList();

            changeSet.DeletedBlogPosts.AddRange(deletedPosts);
        }

        private static void SyncInsertedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet)
        {
            var dbIds = dbPosts.Select(x => x.SourceId).ToList();

            var insertedPosts = sourcePosts.Where(post => !dbIds.Contains(post.SourceId)).Distinct().ToList();

            changeSet.InsertedBlogPosts.AddRange(insertedPosts);
        }

        private static void SyncModifiedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet,
            DateTime? lastUpdatedAt)
        {
            var modifiedPosts =
                dbPosts.Join(
                    sourcePosts,
                    dbPost => dbPost.SourceId,
                    sourcePost => sourcePost.SourceId,
                    (dbPost, sourcePost) => new { DbPost = dbPost, SourcePost = sourcePost }).ToList();

            foreach (var modifiedPost in modifiedPosts)
            {
                var dbPost = modifiedPost.DbPost;
                var sourcePost = modifiedPost.SourcePost;

                bool isModified = (lastUpdatedAt == null) || (dbPost.Hash != sourcePost.Hash);
                if (isModified)
                {
                    changeSet.UpdatedBlogPosts.Add(sourcePost);
                }
            }
        }
    }
}
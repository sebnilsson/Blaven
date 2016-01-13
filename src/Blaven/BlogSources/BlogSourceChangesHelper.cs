using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources
{
    internal static class BlogSourceChangesHelper
    {
        public static BlogSourceChangeSet GetChangeSet(
            ICollection<BlogPost> sourcePosts,
            ICollection<BlogPostBase> dbPosts)
        {
            if (sourcePosts == null)
            {
                throw new ArgumentNullException(nameof(sourcePosts));
            }
            if (dbPosts == null)
            {
                throw new ArgumentNullException(nameof(dbPosts));
            }

            var result = new BlogSourceChangeSet();

            SyncDeletedPosts(sourcePosts, dbPosts, result);
            SyncInsertedPosts(sourcePosts, dbPosts, result);
            SyncModifiedPosts(sourcePosts, dbPosts, result);

            return result;
        }

        private static void SyncDeletedPosts(
            ICollection<BlogPost> sourcePosts,
            ICollection<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet)
        {
            var sourceIds = sourcePosts.Select(x => x.SourceId).ToList();

            var deletedPosts = dbPosts.Where(post => !sourceIds.Contains(post.SourceId)).ToList();

            changeSet.DeletedBlogPosts.AddRange(deletedPosts);
        }

        private static void SyncInsertedPosts(
            ICollection<BlogPost> sourcePosts,
            ICollection<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet)
        {
            var dbIds = dbPosts.Select(x => x.SourceId).ToList();

            var insertedPosts = sourcePosts.Where(post => !dbIds.Contains(post.SourceId)).Distinct().ToList();

            changeSet.InsertedBlogPosts.AddRange(insertedPosts);
        }

        private static void SyncModifiedPosts(
            ICollection<BlogPost> sourcePosts,
            ICollection<BlogPostBase> dbPosts,
            BlogSourceChangeSet changeSet)
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

                bool isModified = (dbPost.Hash != sourcePost.Hash);
                if (isModified)
                {
                    changeSet.UpdatedBlogPosts.Add(sourcePost);
                }
            }
        }
    }
}
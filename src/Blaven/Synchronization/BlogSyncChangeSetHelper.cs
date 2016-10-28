using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Synchronization
{
    internal static class BlogSyncChangeSetHelper
    {
        public static BlogSyncPostsChangeSet GetChangeSet(
            string blogKey,
            IReadOnlyList<BlogPost> sourcePosts,
            IReadOnlyList<BlogPostBase> dataStoragePosts)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }
            if (sourcePosts == null)
            {
                throw new ArgumentNullException(nameof(sourcePosts));
            }
            if (dataStoragePosts == null)
            {
                throw new ArgumentNullException(nameof(dataStoragePosts));
            }

            var cleanSourcePosts =
                sourcePosts.Where(x => x != null)
                    .OrderByDescending(x => x.UpdatedAt)
                    .Distinct(x => x.SourceId)
                    .ToReadOnlyList();

            var cleanDataStoragePosts =
                dataStoragePosts.Where(x => x != null).Distinct(x => x.SourceId).ToReadOnlyList();

            var changeSet = new BlogSyncPostsChangeSet(blogKey);

            SyncDeletedPosts(cleanSourcePosts, cleanDataStoragePosts, changeSet);

            SyncInsertedPosts(cleanSourcePosts, cleanDataStoragePosts, changeSet);

            SyncModifiedPosts(cleanSourcePosts, cleanDataStoragePosts, changeSet);

            return changeSet;
        }

        private static void SyncDeletedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dataStoragePosts,
            BlogSyncPostsChangeSet changeSet)
        {
            var sourceIds = sourcePosts.Select(x => x.SourceId).ToList();

            var deletedPosts = dataStoragePosts.Where(post => !sourceIds.Contains(post.SourceId)).ToList();

            changeSet.DeletedBlogPosts.AddRange(deletedPosts);
        }

        private static void SyncInsertedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dataStoragePosts,
            BlogSyncPostsChangeSet changeSet)
        {
            var dbIds = dataStoragePosts.Select(x => x.SourceId).ToList();

            var insertedPosts = sourcePosts.Where(post => !dbIds.Contains(post.SourceId)).Distinct().ToList();

            changeSet.InsertedBlogPosts.AddRange(insertedPosts);
        }

        private static void SyncModifiedPosts(
            IEnumerable<BlogPost> sourcePosts,
            IEnumerable<BlogPostBase> dataStoragePosts,
            BlogSyncPostsChangeSet changeSet)
        {
            var modifiedPosts =
                dataStoragePosts.Join(
                    sourcePosts,
                    dbPost => dbPost.SourceId,
                    sourcePost => sourcePost.SourceId,
                    (dbPost, sourcePost) => new { DbPost = dbPost, SourcePost = sourcePost }).ToList();

            foreach (var modifiedPost in modifiedPosts)
            {
                var dbPost = modifiedPost.DbPost;
                var sourcePost = modifiedPost.SourcePost;

                bool isModified = (dbPost.Hash != sourcePost.Hash || dbPost.UpdatedAt != sourcePost.UpdatedAt);
                if (isModified)
                {
                    changeSet.UpdatedBlogPosts.Add(sourcePost);
                }
            }
        }
    }
}
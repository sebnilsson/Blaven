using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Synchronization
{
    internal static class BlogPostComparer
    {
        public static SyncBlogPosts Compare(
            IReadOnlyList<BlogPost> blogSourcePosts,
            IReadOnlyList<BlogPostBase> storagePosts)
        {
            if (blogSourcePosts is null)
                throw new ArgumentNullException(nameof(blogSourcePosts));
            if (storagePosts is null)
                throw new ArgumentNullException(nameof(storagePosts));

            var deleted =
                storagePosts
                    .Where(x => !blogSourcePosts.Any(y => y.Id == x.Id))
                    .ToList();

            var inserted =
                blogSourcePosts
                    .Where(x => !storagePosts.Any(y => y.Id == x.Id));

            var updated =
                (from blogSourcePost in blogSourcePosts.Except(inserted)
                 join storagePost in storagePosts
                     on blogSourcePost.Id equals storagePost.Id
                 where !ArePostsEqual(blogSourcePost, storagePost)
                 select blogSourcePost)
                .ToList();

            return new SyncBlogPosts(
                inserted: inserted,
                updated: updated,
                deleted: deleted);
        }

        private static bool ArePostsEqual(
            BlogPost blogSourcePost,
            BlogPostBase storagePost)
        {
            return
                blogSourcePost.Hash == storagePost.Hash
                && blogSourcePost.UpdatedAt == storagePost.UpdatedAt;
        }
    }
}

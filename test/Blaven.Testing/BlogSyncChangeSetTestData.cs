using System.Collections.Generic;
using System.Linq;

using Blaven.Synchronization;

namespace Blaven.Tests
{
    public static class BlogSyncChangeSetTestData
    {
        public static BlogSyncChangeSet CreateWithData(
            string blogKey = null,
            IEnumerable<BlogPostBase> deletedBlogPosts = null,
            IEnumerable<BlogPost> insertedBlogPosts = null,
            IEnumerable<BlogPost> updatedBlogPosts = null)
        {
            blogKey = blogKey ?? BlogMetaTestData.BlogKey;

            var deletedBlogPostList = (deletedBlogPosts ?? Enumerable.Empty<BlogPostBase>()).ToList();
            var insertedBlogPostList = (insertedBlogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
            var updatedBlogPostList = (updatedBlogPosts ?? Enumerable.Empty<BlogPost>()).ToList();

            var changeSet = new BlogSyncChangeSet(blogKey);

            deletedBlogPostList.ForEach(x => changeSet.DeletedBlogPosts.Add(x));
            insertedBlogPostList.ForEach(x => changeSet.InsertedBlogPosts.Add(x));
            updatedBlogPostList.ForEach(x => changeSet.UpdatedBlogPosts.Add(x));

            return changeSet;
        }

        public static BlogSyncChangeSet Create(
            string blogKey = null,
            int deletedBlogPostsCount = 3,
            int insertedBlogPostsCount = 5,
            int updatedBlogPostsCount = 6)
        {
            blogKey = blogKey ?? BlogMetaTestData.BlogKey;

            var changeSet = new BlogSyncChangeSet(blogKey);

            var deletedBlogPosts =
                BlogPostTestData.CreateCollection(0, deletedBlogPostsCount, blogKey).OfType<BlogPostBase>().ToList();

            var insertedBlogPosts =
                Enumerable.Range(0, insertedBlogPostsCount)
                    .Select(i => BlogPostTestData.Create(blogKey, i, isUpdate: false))
                    .ToList();

            int updatedBlogPostsStart = (insertedBlogPostsCount + 1);
            var updatedBlogPosts =
                Enumerable.Range(updatedBlogPostsStart, updatedBlogPostsCount)
                    .Select(i => BlogPostTestData.Create(blogKey, i, isUpdate: true))
                    .ToList();

            changeSet.DeletedBlogPosts.AddRange(deletedBlogPosts);
            changeSet.InsertedBlogPosts.AddRange(insertedBlogPosts);
            changeSet.UpdatedBlogPosts.AddRange(updatedBlogPosts);

            return changeSet;
        }
    }
}
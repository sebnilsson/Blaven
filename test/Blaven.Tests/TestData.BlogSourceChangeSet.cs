using System;
using System.Linq;

using Blaven.BlogSources;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static readonly DateTime TestPublishedAt = new DateTime(2015, 1, 1, 12, 30, 45);

        public static readonly DateTime TestUpdatedAt = new DateTime(2015, 2, 2, 14, 45, 30);

        private static readonly BlavenBlogPostBlavenIdProvider BlavenIdProvider = new BlavenBlogPostBlavenIdProvider();

        public static BlogSourceChangeSet GetBlogSourceChangeSet(
            string blogKey = null,
            int deletedBlogPostsCount = 3,
            int insertedBlogPostsCount = 5,
            int updatedBlogPostsCount = 6)
        {
            var changeSet = new BlogSourceChangeSet();

            var deletedBlogPosts =
                Enumerable.Range(0, deletedBlogPostsCount)
                    .Select(
                        i =>
                        new BlogPostBase
                            {
                                Hash = HashUtility.GetBase64(i),
                                SourceId =
                                    GetTestString(nameof(BlogPostBase.SourceId), blogKey, i, isUpdate: false)
                            })
                    .ToList();

            var insertedBlogPosts =
                Enumerable.Range(0, insertedBlogPostsCount)
                    .Select(i => GetBlogPost(blogKey, i, isUpdate: false))
                    .ToList();

            int updatedBlogPostsStart = (insertedBlogPostsCount + 1);
            var updatedBlogPosts =
                Enumerable.Range(updatedBlogPostsStart, updatedBlogPostsCount)
                    .Select(i => GetBlogPost(blogKey, i, isUpdate: true))
                    .ToList();

            changeSet.DeletedBlogPosts.AddRange(deletedBlogPosts);
            changeSet.InsertedBlogPosts.AddRange(insertedBlogPosts);
            changeSet.UpdatedBlogPosts.AddRange(updatedBlogPosts);

            return changeSet;
        }
    }
}
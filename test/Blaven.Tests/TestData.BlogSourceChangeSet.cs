using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.BlogSources;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static readonly DateTime TestPublishedAt = new DateTime(2015, 1, 1, 12, 30, 45);

        public static readonly DateTime TestUpdatedAt = new DateTime(2015, 2, 2, 14, 45, 30);

        public static BlogSourceChangeSet GetBlogSourceChangeSetWithData(
            string blogKey = null,
            IEnumerable<BlogPostBase> deletedBlogPosts = null,
            IEnumerable<BlogPost> insertedBlogPosts = null,
            IEnumerable<BlogPost> updatedBlogPosts = null)
        {
            blogKey = blogKey ?? BlogKey;
            var deletedBlogPostList = (deletedBlogPosts ?? Enumerable.Empty<BlogPostBase>()).ToList();
            var insertedBlogPostList = (insertedBlogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
            var updatedBlogPostList = (updatedBlogPosts ?? Enumerable.Empty<BlogPost>()).ToList();

            var changeSet = new BlogSourceChangeSet(blogKey);

            deletedBlogPostList.ForEach(x => changeSet.DeletedBlogPosts.Add(x));
            insertedBlogPostList.ForEach(x => changeSet.InsertedBlogPosts.Add(x));
            updatedBlogPostList.ForEach(x => changeSet.UpdatedBlogPosts.Add(x));

            return changeSet;
        }

        public static BlogSourceChangeSet GetBlogSourceChangeSet(
            string blogKey = null,
            int deletedBlogPostsCount = 3,
            int insertedBlogPostsCount = 5,
            int updatedBlogPostsCount = 6)
        {
            blogKey = blogKey ?? BlogKey;

            var changeSet = new BlogSourceChangeSet(blogKey);

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

            deletedBlogPosts.ForEach(x => x.BlavenId = BlavenBlogPostBlavenIdProvider.GetId(x.SourceId));

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
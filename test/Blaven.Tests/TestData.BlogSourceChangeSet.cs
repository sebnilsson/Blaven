using System.Collections.Generic;
using System.Linq;

using Blaven.Synchronization;

namespace Blaven.Tests
{
    public static partial class TestData
    {
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

        public static IReadOnlyList<BlogPost> GetBlogPosts(string blogKey = null, int blogPostsCount = 11)
        {
            blogKey = blogKey ?? BlogKey;

            var blogPosts =
                Enumerable.Range(0, blogPostsCount)
                    .Select(i => GetBlogPost(blogKey, i, isUpdate: (i % 2 == 0)))
                    .ToList();
            return blogPosts;
        }

        public static BlogSourceChangeSet GetBlogSourceChangeSet(
            string blogKey = null,
            int deletedBlogPostsCount = 3,
            int insertedBlogPostsCount = 5,
            int updatedBlogPostsCount = 6)
        {
            blogKey = blogKey ?? BlogKey;

            var changeSet = new BlogSourceChangeSet(blogKey);

            var deletedBlogPosts = GetBlogPosts(0, deletedBlogPostsCount, blogKey).OfType<BlogPostBase>().ToList();

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
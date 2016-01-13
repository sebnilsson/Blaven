using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Tests;
using Google.Apis.Blogger.v3.Data;
using Xunit;

namespace Blaven.BlogSources.Blogger.Tests
{
    public class BloggerBlogSourceTest
    {
        private const string TestBlogId = "TestBlogId";

        private const string TestBlogName = "TestBlogName";

        [Fact]
        public void GetMeta_BloggerApiProviderContainsData_ShouldReturnAllFields()
        {
            var bloggerBlogSource = GetTestBloggerBlogSource(getBlogFunc: _ => BloggerTestData.GetBlog());
            var blogSetting = GetTestBlogSetting();

            var meta = bloggerBlogSource.GetMeta(blogSetting);

            Assert.NotNull(meta);
            Assert.Equal(blogSetting.BlogKey, meta.BlogKey);
            Assert.Equal(BloggerTestData.BlogDescription, meta.Description);
            Assert.Equal(BloggerTestData.BlogName, meta.Name);
            Assert.Equal(BloggerTestData.BlogPublishedAt, meta.PublishedAt);
            Assert.Equal(BloggerTestData.BlogId, meta.SourceId);
            Assert.Equal(BloggerTestData.BlogUpdatedAt, meta.UpdatedAt);
            Assert.Equal(BloggerTestData.BlogUrl, meta.Url);
        }

        [Fact]
        public void GetChanges_BloggerApiProviderContainsDataAndEmptyDb_ReturnsInsertWithAllFields()
        {
            var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts());
            var blogSetting = GetTestBlogSetting();
            var dbPosts = Enumerable.Empty<BlogPostBase>();

            var changes = bloggerBlogSource.GetChanges(blogSetting, dbPosts);

            var inserted = changes.InsertedBlogPosts.FirstOrDefault();

            Assert.NotNull(inserted);
            Assert.Equal(blogSetting.BlogKey, inserted.BlogKey);
            Assert.Equal(BloggerTestData.AuthorId, inserted.Author.SourceId);
            Assert.Equal(BloggerTestData.AuthorImageUrl, inserted.Author.ImageUrl);
            Assert.Equal(BloggerTestData.AuthorDisplayName, inserted.Author.Name);
            Assert.Equal(BloggerTestData.AuthorUrl, inserted.Author.Url);
            Assert.Equal(BloggerTestData.PostContent, inserted.Content);
            Assert.Equal(BloggerTestData.PostId, inserted.SourceId);
            Assert.Equal(BloggerTestData.PostTitle, inserted.Title);
            Assert.Equal(BloggerTestData.PostUrl, inserted.SourceUrl);
            bool tagsAreSequenceEquals =
                BloggerTestData.PostTags.OrderBy(x => x).SequenceEqual(inserted.Tags.OrderBy(x => x));
            Assert.True(tagsAreSequenceEquals);
        }

        [Fact]
        public void GetChanges_BlogSourceInsertedPosts_ReturnsInsertedPosts()
        {
            var dbPosts = GetTestBlogPosts(0, 1).ToList();

            var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts(0, 3));

            var changes = GetTestBlogSourceChangeSet(bloggerBlogSource, dbPosts);

            Assert.Equal(2, changes.InsertedBlogPosts.Count);
        }

        [Fact]
        public void GetChanges_BlogSourceDeletedPosts_ReturnsDeletedPosts()
        {
            var dbPosts = GetTestBlogPosts(0, 3).ToList();

            var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => BloggerTestData.GetPosts(0, 1));

            var changes = GetTestBlogSourceChangeSet(bloggerBlogSource, dbPosts);

            Assert.Equal(2, changes.DeletedBlogPosts.Count);
        }

        [Fact]
        public void GetChanges_BlogSourceUpdatedPosts_ReturnsUpdatedPosts()
        {
            var dbPosts = GetTestBlogPosts(0, 2).ToList();

            var bloggerBlogSource = GetTestBloggerBlogSource(getPostsFunc: _ => GetTestModifiedPosts(0, 2));

            var changes = GetTestBlogSourceChangeSet(bloggerBlogSource, dbPosts);

            Assert.Equal(2, changes.UpdatedBlogPosts.Count);
        }

        private static IEnumerable<Post> GetTestModifiedPosts(int start, int count, string blogKey = TestData.BlogKey)
        {
            var posts = BloggerTestData.GetPosts(start, count, blogKey).Where(x => x.Updated.HasValue);
            foreach (var post in posts)
            {
                if (post.Updated.HasValue)
                {
                    post.Updated = post.Updated.Value.AddDays(1);
                }

                yield return post;
            }
        }

        private static BlogSourceChangeSet GetTestBlogSourceChangeSet(
            IBlogSource bloggerBlogSource,
            IEnumerable<BlogPostBase> dbPosts)
        {
            var blogSetting = GetTestBlogSetting();

            var changes = bloggerBlogSource.GetChanges(blogSetting, dbPosts);
            return changes;
        }

        private static IEnumerable<BlogPostBase> GetTestBlogPosts(
            int start,
            int count,
            string blogKey = TestData.BlogKey)
        {
            var posts = BloggerTestData.GetPosts(start, count, blogKey);

            var blogPosts = posts.Select(BloggerDataConverter.ConvertPost);
            return blogPosts;
        }

        private static BloggerBlogSource GetTestBloggerBlogSource(
            Func<string, Blog> getBlogFunc = null,
            Func<string, IEnumerable<Post>> getPostsFunc = null)
        {
            var bloggerApiProvider = new MockBloggerApiProvider(getBlogFunc, getPostsFunc);

            var bloggerBlogSource = new BloggerBlogSource(bloggerApiProvider);
            return bloggerBlogSource;
        }

        private static BlogSetting GetTestBlogSetting()
        {
            var blogSettings = new BlogSetting(TestData.BlogKey, TestBlogId, TestBlogName);
            return blogSettings;
        }
    }
}
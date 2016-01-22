using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Blaven.Tests;

namespace Blaven.Data.Tests
{
    public class MockRepository : IRepository
    {
        private readonly Func<string, BlogMeta> getBlogMetaFunc;

        private readonly Func<string, string, BlogPost> getPostFunc;

        private readonly Func<string, string, BlogPost> getPostBySourceIdFunc;

        private readonly Func<IEnumerable<string>, IQueryable<BlogArchiveItem>> listAllArchiveFunc;

        private readonly Func<IEnumerable<string>, IQueryable<BlogTagItem>> listAllTagsFunc;

        private readonly Func<IEnumerable<string>, IQueryable<BlogPostHead>> listPostHeadsFunc;

        private readonly Func<IEnumerable<string>, IQueryable<BlogPost>> listPostsFunc;

        private readonly Func<IEnumerable<string>, DateTime, IQueryable<BlogPost>> listPostsByArchiveFunc;

        private readonly Func<IEnumerable<string>, string, IQueryable<BlogPost>> listPostsByTagFunc;

        public MockRepository(
            Func<string, BlogMeta> getBlogMetaFunc = null,
            Func<string, string, BlogPost> getPostFunc = null,
            Func<string, string, BlogPost> getPostBySourceIdFunc = null,
            Func<IEnumerable<string>, IQueryable<BlogArchiveItem>> listAllArchiveFunc = null,
            Func<IEnumerable<string>, IQueryable<BlogTagItem>> listAllTagsFunc = null,
            Func<IEnumerable<string>, IQueryable<BlogPostHead>> listPostHeadsFunc = null,
            Func<IEnumerable<string>, IQueryable<BlogPost>> listPostsFunc = null,
            Func<IEnumerable<string>, DateTime, IQueryable<BlogPost>> listPostsByArchiveFunc = null,
            Func<IEnumerable<string>, string, IQueryable<BlogPost>> listPostsByTagFunc = null)
        {
            this.getBlogMetaFunc = (getBlogMetaFunc ?? (_ => null)).WithTracking(this.GetBlogMetaTracker);
            this.getPostFunc = (getPostFunc ?? ((_, __) => null)).WithTracking(this.GetPostTracker);
            this.getPostBySourceIdFunc =
                (getPostBySourceIdFunc ?? ((_, __) => null)).WithTracking(this.GetPostBySourceIdTracker);
            this.listAllArchiveFunc = (listAllArchiveFunc ?? (_ => null)).WithTracking(this.ListAllArchiveTracker);
            this.listAllTagsFunc = (listAllTagsFunc ?? (_ => null)).WithTracking(this.ListAllTagsTracker);
            this.listPostHeadsFunc = (listPostHeadsFunc ?? (_ => null)).WithTracking(this.ListPostHeadsTracker);
            this.listPostsFunc = (listPostsFunc ?? (_ => null)).WithTracking(this.ListPostsTracker);
            this.listPostsByArchiveFunc =
                (listPostsByArchiveFunc ?? ((_, __) => null)).WithTracking(this.ListPostsByArchiveTracker);
            this.listPostsByTagFunc = (listPostsByTagFunc ?? ((_, __) => null)).WithTracking(this.ListPostsByTagTracker);
        }

        public DelegateTracker<string> GetBlogMetaTracker { get; } = new DelegateTracker<string>();

        public DelegateTracker<string> GetPostTracker { get; } = new DelegateTracker<string>();

        public DelegateTracker<string> GetPostBySourceIdTracker { get; } = new DelegateTracker<string>();

        public DelegateTracker<IEnumerable<string>> ListAllArchiveTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public DelegateTracker<IEnumerable<string>> ListAllTagsTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public DelegateTracker<IEnumerable<string>> ListPostHeadsTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public DelegateTracker<IEnumerable<string>> ListPostsTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public DelegateTracker<IEnumerable<string>> ListPostsByArchiveTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public DelegateTracker<IEnumerable<string>> ListPostsByTagTracker { get; } =
            new DelegateTracker<IEnumerable<string>>();

        public BlogMeta GetBlogMeta(string blogKey)
        {
            var meta = this.getBlogMetaFunc?.Invoke(blogKey);
            return meta;
        }

        public BlogPost GetPost(string blogKey, string blavenId)
        {
            var post = this.getPostFunc?.Invoke(blogKey, blavenId);
            return post;
        }

        public BlogPost GetPostBySourceId(string blogKey, string sourceId)
        {
            var post = this.getPostBySourceIdFunc?.Invoke(blogKey, sourceId);
            return post;
        }

        public IQueryable<BlogArchiveItem> ListArchive(IEnumerable<string> blogKeys)
        {
            var archive = this.listAllArchiveFunc?.Invoke(blogKeys);
            return archive;
        }

        public IQueryable<BlogPostHead> ListPostHeads(IEnumerable<string> blogKeys)
        {
            var postHeads = this.listPostHeadsFunc?.Invoke(blogKeys);
            return postHeads;
        }

        public IQueryable<BlogPost> ListPosts(IEnumerable<string> blogKeys)
        {
            var posts = this.listPostsFunc?.Invoke(blogKeys);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByArchive(IEnumerable<string> blogKeys, DateTime date)
        {
            var posts = this.listPostsByArchiveFunc?.Invoke(blogKeys, date);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByTag(IEnumerable<string> blogKeys, string tagName)
        {
            var posts = this.listPostsByTagFunc?.Invoke(blogKeys, tagName);
            return posts;
        }

        public IQueryable<BlogTagItem> ListTags(IEnumerable<string> blogKeys)
        {
            var tags = this.listAllTagsFunc?.Invoke(blogKeys);
            return tags;
        }

        public static MockRepository Create(
            IEnumerable<BlogPost> blogPosts = null,
            IEnumerable<BlogMeta> blogMetas = null,
            int funcSleep = 100)
        {
            var blogPostList = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToReadOnlyList();
            var blogMetaList = (blogMetas ?? Enumerable.Empty<BlogMeta>()).ToReadOnlyList();

            var blogSource = new MockRepository(
                getBlogMetaFunc: blogKey =>
                    {
                        Thread.Sleep(funcSleep);
                        return blogMetaList.FirstOrDefault(x => x.BlogKey == blogKey);
                    },
                getPostFunc: (blogKey, blavenId) =>
                    {
                        Thread.Sleep(funcSleep);
                        return blogPostList.FirstOrDefault(x => x.BlogKey == blogKey && x.BlavenId == blavenId);
                    },
                getPostBySourceIdFunc: (blogKey, sourceId) =>
                    {
                        Thread.Sleep(funcSleep);
                        return blogPostList.FirstOrDefault(x => x.BlogKey == blogKey && x.SourceId == sourceId);
                    },
                listAllArchiveFunc: blogKeys =>
                    {
                        Thread.Sleep(funcSleep);
                        var archive = from post in blogPostList
                                      where post.PublishedAt.HasValue && blogKeys.Contains(post.BlogKey)
                                      group post by new { post.PublishedAt.Value.Year, post.PublishedAt.Value.Month }
                                      into g
                                      select
                                          new BlogArchiveItem
                                              {
                                                  Date = new DateTime(g.Key.Year, g.Key.Month, 1),
                                                  Count = g.Count()
                                              };
                        return archive.AsQueryable();
                    },
                listAllTagsFunc: blogKeys =>
                    {
                        Thread.Sleep(funcSleep);
                        var tags = from post in blogPostList
                                   where post.Tags != null && blogKeys.Contains(post.BlogKey)
                                   from tag in post.Tags
                                   group tag by tag
                                   into g
                                   select new BlogTagItem { Name = g.Key, Count = g.Count() };
                        return tags.AsQueryable();
                    },
                listPostHeadsFunc: blogKeys =>
                    {
                        Thread.Sleep(funcSleep);
                        var postHeads = blogPostList.Where(x => blogKeys.Contains(x.BlogKey));
                        return postHeads.AsQueryable();
                    },
                listPostsFunc: blogKeys =>
                    {
                        Thread.Sleep(funcSleep);
                        var posts = blogPostList.Where(x => blogKeys.Contains(x.BlogKey));
                        return posts.AsQueryable();
                    },
                listPostsByArchiveFunc: (blogKeys, date) =>
                    {
                        Thread.Sleep(funcSleep);
                        var posts =
                            blogPostList.Where(
                                x =>
                                blogKeys.Contains(x.BlogKey) && x.PublishedAt.HasValue
                                && x.PublishedAt.Value.Year == date.Year && x.PublishedAt.Value.Month == date.Month);
                        return posts.AsQueryable();
                    },
                listPostsByTagFunc: (blogKeys, tagName) =>
                    {
                        Thread.Sleep(funcSleep);
                        var posts =
                            blogPostList.Where(
                                x =>
                                blogKeys.Contains(x.BlogKey) && x.Tags != null
                                && x.Tags.Contains(tagName, StringComparer.InvariantCultureIgnoreCase));
                        return posts.AsQueryable();
                    });
            return blogSource;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Synchronization;
using Blaven.Tests;

namespace Blaven.BlogSources.Tests
{
    [DebuggerDisplay(
         "GetMetaTracker={GetMetaTracker.Events.Count}, " + "GetChangesTracker={GetChangesTracker.Events.Count}")]
    public class MockBlogSource : IBlogSource
    {
        private readonly Func<BlogSetting, BlogMeta> getMetaFunc;

        private readonly Func<BlogSetting, DateTime?, IEnumerable<BlogPostBase>, IReadOnlyList<BlogPost>> getBlogPosts;

        public MockBlogSource(
            Func<BlogSetting, BlogMeta> getMetaFunc = null,
            Func<BlogSetting, DateTime?, IEnumerable<BlogPostBase>, IReadOnlyList<BlogPost>> getBlogPosts = null)
        {
            this.getMetaFunc = (getMetaFunc ?? (_ => null)).WithTracking(this.GetMetaTracker);
            this.getBlogPosts = (getBlogPosts ?? ((_, __, ___) => null)).WithTracking(this.GetChangesTracker);
        }

        public DelegateTracker<BlogSetting> GetMetaTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> GetChangesTracker { get; } = new DelegateTracker<BlogSetting>();

        public async Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var meta = this.getMetaFunc?.Invoke(blogSetting);
            return await Task.FromResult(meta);
        }

        public async Task<IReadOnlyList<BlogPost>> GetBlogPosts(
            BlogSetting blogSetting,
            IEnumerable<BlogPostBase> dataStoragePosts,
            DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (dataStoragePosts == null)
            {
                throw new ArgumentNullException(nameof(dataStoragePosts));
            }

            var changes = this.getBlogPosts?.Invoke(blogSetting, lastUpdatedAt, dataStoragePosts);
            return await Task.FromResult(changes);
        }

        public static MockBlogSource Create(int getMetaFuncSleep = 100, int getChangesFuncSleep = 100)
        {
            var blogSource = new MockBlogSource(
                                 getMetaFunc: blogSetting =>
                                     {
                                         Thread.Sleep(getMetaFuncSleep);
                                         return TestData.GetBlogMeta(blogSetting.BlogKey);
                                     },
                                 getBlogPosts: (blogSetting, __, ___) =>
                                     {
                                         Thread.Sleep(getChangesFuncSleep);
                                         return TestData.GetBlogPosts(blogSetting.BlogKey);
                                     });
            return blogSource;
        }
    }
}
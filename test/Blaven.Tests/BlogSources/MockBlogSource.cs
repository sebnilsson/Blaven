using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Blaven.Tests;

namespace Blaven.BlogSources.Tests
{
    [DebuggerDisplay(
        "GetMetaTracker={GetMetaTracker.Events.Count}, " + "GetChangesTracker={GetChangesTracker.Events.Count}")]
    public class MockBlogSource : IBlogSource
    {
        private readonly Func<BlogSetting, BlogMeta> getMetaFunc;

        private readonly Func<BlogSetting, DateTime?, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc;

        public MockBlogSource(
            Func<BlogSetting, BlogMeta> getMetaFunc = null,
            Func<BlogSetting, DateTime?, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc = null)
        {
            this.getMetaFunc = (getMetaFunc ?? (_ => null)).WithTracking(this.GetMetaTracker);
            this.getChangesFunc = (getChangesFunc ?? ((_, __, ___) => null)).WithTracking(this.GetChangesTracker);
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

        public async Task<BlogSourceChangeSet> GetChanges(
            BlogSetting blogSetting,
            IEnumerable<BlogPostBase> dbPosts,
            DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (dbPosts == null)
            {
                throw new ArgumentNullException(nameof(dbPosts));
            }

            var changes = this.getChangesFunc?.Invoke(blogSetting, lastUpdatedAt, dbPosts);
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
                getChangesFunc: (blogSetting, __, ___) =>
                    {
                        Thread.Sleep(getChangesFuncSleep);
                        return TestData.GetBlogSourceChangeSet(blogSetting.BlogKey);
                    });
            return blogSource;
        }
    }
}
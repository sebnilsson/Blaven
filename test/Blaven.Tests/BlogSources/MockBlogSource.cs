using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Blaven.Tests;

namespace Blaven.BlogSources.Tests
{
    [DebuggerDisplay(
        "GetMetaTracker={GetMetaTracker.Events.Count}, " + "GetChangesTracker={GetChangesTracker.Events.Count}")]
    public class MockBlogSource : IBlogSource
    {
        private readonly Func<BlogSetting, BlogMeta> getMetaFunc;

        private readonly Func<BlogSetting, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc;

        public MockBlogSource(
            Func<BlogSetting, BlogMeta> getMetaFunc = null,
            Func<BlogSetting, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc = null)
        {
            this.getMetaFunc = (getMetaFunc ?? (_ => null)).WithTracking(this.GetMetaTracker);
            this.getChangesFunc = (getChangesFunc ?? ((_, __) => null)).WithTracking(this.GetChangesTracker);
        }

        public DelegateTracker<BlogSetting> GetMetaTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> GetChangesTracker { get; } = new DelegateTracker<BlogSetting>();

        public BlogMeta GetMeta(BlogSetting blogSetting, DateTime lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var meta = this.getMetaFunc?.Invoke(blogSetting);
            return meta;
        }

        public BlogSourceChangeSet GetChanges(
            BlogSetting blogSetting,
            DateTime lastUpdatedAt,
            IEnumerable<BlogPostBase> dbPosts)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (dbPosts == null)
            {
                throw new ArgumentNullException(nameof(dbPosts));
            }

            var changes = this.getChangesFunc?.Invoke(blogSetting, dbPosts);
            return changes;
        }

        public static MockBlogSource Create(int getMetaFuncSleep = 100, int getChangesFuncSleep = 100)
        {
            var blogSource = new MockBlogSource(
                getMetaFunc: blogSetting =>
                    {
                        Thread.Sleep(getMetaFuncSleep);
                        return TestData.GetBlogMeta(blogSetting.BlogKey);
                    },
                getChangesFunc: (blogSetting, __) =>
                    {
                        Thread.Sleep(getChangesFuncSleep);
                        return TestData.GetBlogSourceChangeSet(blogSetting.BlogKey);
                    });
            return blogSource;
        }
    }
}
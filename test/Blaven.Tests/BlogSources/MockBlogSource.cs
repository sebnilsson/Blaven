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

        public BlogMeta GetMeta(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            return this.getMetaFunc?.Invoke(blogSetting);
        }

        public DelegateTracker<BlogSetting> GetMetaTracker { get; } = new DelegateTracker<BlogSetting>();

        public BlogSourceChangeSet GetChanges(BlogSetting blogSetting, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (dbBlogPosts == null)
            {
                throw new ArgumentNullException(nameof(dbBlogPosts));
            }

            return this.getChangesFunc?.Invoke(blogSetting, dbBlogPosts);
        }

        public DelegateTracker<BlogSetting> GetChangesTracker { get; } = new DelegateTracker<BlogSetting>();

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
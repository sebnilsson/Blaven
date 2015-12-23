using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Blaven.Tests;

namespace Blaven.BlogSources.Tests
{
    [DebuggerDisplay(
        "GetMetaTracker={GetMetaTracker.Events.Count}, " + "GetChangesTracker={GetChangesTracker.Events.Count}")]
    public class TestBlogSource : IBlogSource
    {
        private readonly Func<string, BlogMeta> getMetaFunc;

        private readonly Func<string, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc;

        public TestBlogSource(
            Func<string, BlogMeta> getMetaFunc = null,
            Func<string, IEnumerable<BlogPostBase>, BlogSourceChangeSet> getChangesFunc = null)
        {
            this.getMetaFunc = (getMetaFunc ?? (_ => null)).WithTracking(this.GetMetaTracker);
            this.getChangesFunc = (getChangesFunc ?? ((_, __) => null)).WithTracking(this.GetChangesTracker);
        }

        public BlogMeta GetMeta(string blogKey)
        {
            return this.getMetaFunc?.Invoke(blogKey);
        }

        public DelegateTracker<string> GetMetaTracker { get; } = new DelegateTracker<string>();

        public BlogSourceChangeSet GetChanges(string blogKey, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            return this.getChangesFunc?.Invoke(blogKey, dbBlogPosts);
        }

        public DelegateTracker<string> GetChangesTracker { get; } = new DelegateTracker<string>();

        public static TestBlogSource Create(int getMetaFuncSleep = 100, int getChangesFuncSleep = 100)
        {
            var blogSource = new TestBlogSource(
                getMetaFunc: blogKey =>
                    {
                        Thread.Sleep(getMetaFuncSleep);
                        return TestData.GetBlogMeta(blogKey);
                    },
                getChangesFunc: (blogKey, __) =>
                    {
                        Thread.Sleep(getChangesFuncSleep);
                        return TestData.GetBlogSourceChangeSet(blogKey);
                    });
            return blogSource;
        }
    }
}
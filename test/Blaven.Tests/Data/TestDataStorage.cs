using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Blaven.BlogSources;
using Blaven.Tests;

namespace Blaven.Data.Tests
{
    [DebuggerDisplay(
        "GetBlogPostsTracker={GetBlogPostsTracker.Events.Count}, "
        + "SaveBlogMetaTracker={SaveBlogMetaTracker.Events.Count}, "
        + "SaveChangesTracker={SaveChangesTracker.Events.Count}")]
    public class TestDataStorage : IDataStorage
    {
        private readonly Func<string, IEnumerable<BlogPostBase>> getBlogPostsFunc;

        private readonly Action<string, BlogMeta> saveBlogMetaAction;

        private readonly Action<string, BlogSourceChangeSet> saveChangesAction;

        public TestDataStorage(
            Func<string, IEnumerable<BlogPostBase>> getBlogPostsFunc = null,
            Action<string, BlogMeta> saveBlogMetaAction = null,
            Action<string, BlogSourceChangeSet> saveChangesAction = null)
        {
            this.getBlogPostsFunc = (getBlogPostsFunc ?? (_ => null)).WithTracking(this.GetBlogPostsTracker);
            this.saveBlogMetaAction = (saveBlogMetaAction ?? ((_, __) => { })).WithTracking(this.SaveBlogMetaTracker);
            this.saveChangesAction = (saveChangesAction ?? ((_, __) => { })).WithTracking(this.SaveChangesTracker);
        }

        public IEnumerable<BlogPostBase> GetBlogPosts(string blogKey)
        {
            return this.getBlogPostsFunc?.Invoke(blogKey);
        }

        public DelegateTracker<string> GetBlogPostsTracker { get; } = new DelegateTracker<string>();

        public void SaveBlogMeta(string blogKey, BlogMeta blogMeta)
        {
            this.saveBlogMetaAction?.Invoke(blogKey, blogMeta);
        }

        public DelegateTracker<string> SaveBlogMetaTracker { get; } = new DelegateTracker<string>();

        public void SaveChanges(string blogKey, BlogSourceChangeSet changeSet)
        {
            this.saveChangesAction?.Invoke(blogKey, changeSet);
        }

        public DelegateTracker<string> SaveChangesTracker { get; } = new DelegateTracker<string>();

        public static TestDataStorage Create(
            int getBlogPostsFuncSleep = 100,
            int saveBlogMetaActionSleep = 100,
            int saveChangesActionSleep = 100)
        {
            var dataStorage = new TestDataStorage(
                getBlogPostsFunc: blogKey =>
                    {
                        Thread.Sleep(getBlogPostsFuncSleep);
                        return TestData.GetBlogPostBases(blogKey, blogPostCount: 10, blogPostStart: 100);
                    },
                saveBlogMetaAction: (_, __) => { Thread.Sleep(saveBlogMetaActionSleep); },
                saveChangesAction: (_, __) => { Thread.Sleep(saveChangesActionSleep); });
            return dataStorage;
        }
    }
}
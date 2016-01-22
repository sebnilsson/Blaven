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
    public class MockDataStorage : IDataStorage
    {
        private readonly Func<BlogSetting, IReadOnlyCollection<BlogPostBase>> getBlogPostsFunc;

        private readonly Action<BlogSetting, BlogMeta> saveBlogMetaAction;

        private readonly Action<BlogSetting, BlogSourceChangeSet> saveChangesAction;

        public MockDataStorage(
            Func<BlogSetting, IReadOnlyCollection<BlogPostBase>> getBlogPostsFunc = null,
            Action<BlogSetting, BlogMeta> saveBlogMetaAction = null,
            Action<BlogSetting, BlogSourceChangeSet> saveChangesAction = null)
        {
            this.getBlogPostsFunc = (getBlogPostsFunc ?? (_ => null)).WithTracking(this.GetBlogPostsTracker);
            this.saveBlogMetaAction = (saveBlogMetaAction ?? ((_, __) => { })).WithTracking(this.SaveBlogMetaTracker);
            this.saveChangesAction = (saveChangesAction ?? ((_, __) => { })).WithTracking(this.SaveChangesTracker);
        }

        public DelegateTracker<BlogSetting> GetBlogPostsTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> SaveBlogMetaTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> SaveChangesTracker { get; } = new DelegateTracker<BlogSetting>();

        public IReadOnlyCollection<BlogPostBase> GetPostBases(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            return this.getBlogPostsFunc?.Invoke(blogSetting);
        }

        public void SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (blogMeta == null)
            {
                throw new ArgumentNullException(nameof(blogMeta));
            }

            this.saveBlogMetaAction?.Invoke(blogSetting, blogMeta);
        }

        public void SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (changeSet == null)
            {
                throw new ArgumentNullException(nameof(changeSet));
            }

            this.saveChangesAction?.Invoke(blogSetting, changeSet);
        }

        public static MockDataStorage Create(
            int getBlogPostsFuncSleep = 100,
            int saveBlogMetaActionSleep = 100,
            int saveChangesActionSleep = 100)
        {
            var dataStorage = new MockDataStorage(
                getBlogPostsFunc: blogSetting =>
                    {
                        Thread.Sleep(getBlogPostsFuncSleep);
                        return
                            TestData.GetBlogPostBases(blogSetting.BlogKey, blogPostCount: 10, blogPostStart: 100)
                                .ToReadOnlyList();
                    },
                saveBlogMetaAction: (_, __) => { Thread.Sleep(saveBlogMetaActionSleep); },
                saveChangesAction: (_, __) => { Thread.Sleep(saveChangesActionSleep); });
            return dataStorage;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly Func<BlogSetting, IReadOnlyList<BlogPostBase>> getPostBasesFunc;

        private readonly Func<BlogSetting, DateTime?> getLastPostUpdatedAtFunc;

        private readonly Action<BlogSetting, BlogMeta> saveBlogMetaAction;

        private readonly Action<BlogSetting, BlogSourceChangeSet> saveChangesAction;

        public MockDataStorage(
            Func<BlogSetting, IReadOnlyList<BlogPostBase>> getPostBasesFunc = null,
            Action<BlogSetting, BlogMeta> saveBlogMetaAction = null,
            Action<BlogSetting, BlogSourceChangeSet> saveChangesAction = null,
            Func<BlogSetting, DateTime?> getLastPostUpdatedAtFunc = null)
        {
            this.getPostBasesFunc = (getPostBasesFunc ?? (_ => null)).WithTracking(this.GetBlogPostsTracker);
            this.getLastPostUpdatedAtFunc =
                (getLastPostUpdatedAtFunc ?? (_ => null)).WithTracking(this.GetLastPostUpdatedAtTracker);
            this.saveBlogMetaAction = (saveBlogMetaAction ?? ((_, __) => { })).WithTracking(this.SaveBlogMetaTracker);
            this.saveChangesAction = (saveChangesAction ?? ((_, __) => { })).WithTracking(this.SaveChangesTracker);
        }

        public DelegateTracker<BlogSetting> GetLastPostUpdatedAtTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> GetBlogPostsTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> SaveBlogMetaTracker { get; } = new DelegateTracker<BlogSetting>();

        public DelegateTracker<BlogSetting> SaveChangesTracker { get; } = new DelegateTracker<BlogSetting>();

        public async Task<DateTime?> GetLastUpdatedAt(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            return await Task.FromResult(this.getLastPostUpdatedAtFunc?.Invoke(blogSetting));
        }

        public async Task<IReadOnlyList<BlogPostBase>> GetPostBases(
            BlogSetting blogSetting,
            DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            return await Task.FromResult(this.getPostBasesFunc?.Invoke(blogSetting));
        }

        public async Task SaveBlogMeta(BlogSetting blogSetting, BlogMeta blogMeta)
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

            await Task.CompletedTask;
        }

        public async Task SaveChanges(BlogSetting blogSetting, BlogSourceChangeSet changeSet)
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

            await Task.CompletedTask;
        }

        public static MockDataStorage Create(
            int getBlogPostsFuncSleep = 100,
            int getLastPostUpdatedAtFuncSleep = 100,
            int saveBlogMetaActionSleep = 100,
            int saveChangesActionSleep = 100)
        {
            var dataStorage = new MockDataStorage(
                                  getPostBasesFunc: blogSetting =>
                                      {
                                          Thread.Sleep(getBlogPostsFuncSleep);
                                          return
                                              TestData.GetBlogPostBases(
                                                  start: 10,
                                                  count: 100,
                                                  blogKey: blogSetting.BlogKey).ToReadOnlyList();
                                      },
                                  getLastPostUpdatedAtFunc: blogSetting =>
                                      {
                                          Thread.Sleep(getLastPostUpdatedAtFuncSleep);
                                          return
                                              TestData.GetBlogPosts(start: 100, count: 10, blogKey: blogSetting.BlogKey)
                                                  .Select(x => x.PublishedAt)
                                                  .OrderByDescending(x => x)
                                                  .FirstOrDefault();
                                      },
                                  saveBlogMetaAction: (_, __) => { Thread.Sleep(saveBlogMetaActionSleep); },
                                  saveChangesAction: (_, __) => { Thread.Sleep(saveChangesActionSleep); });
            return dataStorage;
        }
    }
}
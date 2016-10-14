using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.BlogSources
{
    public abstract class BlogSourceBase : IBlogSource
    {
        public virtual async Task<BlogSourceChangeSet> GetChanges(
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

            var dbBlogPostsList = dbPosts.ToList();

            var sourcePosts = await this.GetSourcePosts(blogSetting, lastUpdatedAt);

            var changeSet = BlogSourceChangesHelper.GetChangeSet(
                blogSetting.BlogKey,
                sourcePosts.ToList(),
                dbBlogPostsList,
                lastUpdatedAt);
            return changeSet;
        }

        public abstract Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt);

        protected abstract Task<IEnumerable<BlogPost>> GetSourcePosts(BlogSetting blogSetting, DateTime? lastUpdatedAt);
    }
}
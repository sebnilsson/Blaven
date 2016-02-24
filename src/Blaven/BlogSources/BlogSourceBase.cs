using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources
{
    public abstract class BlogSourceBase : IBlogSource
    {
        public virtual BlogSourceChangeSet GetChanges(
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

            var dbBlogPostsList = dbPosts.ToList();

            var sourcePosts = this.GetSourcePosts(blogSetting, lastUpdatedAt).ToList();

            var changeSet = BlogSourceChangesHelper.GetChangeSet(blogSetting.BlogKey, sourcePosts, dbBlogPostsList);
            return changeSet;
        }

        public abstract BlogMeta GetMeta(BlogSetting blogSetting, DateTime lastUpdatedAt);

        protected abstract IEnumerable<BlogPost> GetSourcePosts(BlogSetting blogSetting, DateTime lastUpdatedAt);
    }
}
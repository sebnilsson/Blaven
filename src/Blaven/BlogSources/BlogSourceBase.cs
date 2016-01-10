using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources
{
    public abstract class BlogSourceBase : IBlogSource
    {
        public abstract BlogMeta GetMeta(BlogSetting blogSetting);

        public virtual BlogSourceChangeSet GetChanges(BlogSetting blogSetting, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }
            if (dbBlogPosts == null)
            {
                throw new ArgumentNullException(nameof(dbBlogPosts));
            }

            var sourceBlogPosts = this.GetSourceBlogPosts().ToList();

            var dbBlogPostList = dbBlogPosts.ToList();

            var changeSet = BlogSourceChangeHelper.GetChangeSet(sourceBlogPosts, dbBlogPostList);
            return changeSet;
        }

        protected abstract IEnumerable<BlogPost> GetSourceBlogPosts();
    }
}
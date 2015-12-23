using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.BlogSources
{
    public abstract class BlogSourceBase : IBlogSource
    {
        public abstract BlogMeta GetMeta(string blogKey);

        public virtual BlogSourceChangeSet GetChanges(string blogKey, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
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
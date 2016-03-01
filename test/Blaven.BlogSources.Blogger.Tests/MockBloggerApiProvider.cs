using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Blaven.Tests;
using Google.Apis.Blogger.v3.Data;

namespace Blaven.BlogSources.Blogger.Tests
{
    [DebuggerDisplay(
        "GetBlogPostsTracker={GetBlogTracker.Events.Count}, " + "SaveBlogMetaTracker={GetPostsTracker.Events.Count}, "
        + "SaveChangesTracker={GetPostsSlimTracker.Events.Count}")]
    public class MockBloggerApiProvider : BloggerApiProvider
    {
        private readonly Func<string, Blog> getBlogFunc;

        private readonly Func<string, IEnumerable<Post>> getPostsFunc;

        public MockBloggerApiProvider(
            Func<string, Blog> getBlogFunc = null,
            Func<string, IEnumerable<Post>> getPostsFunc = null)
        {
            this.getBlogFunc = (getBlogFunc ?? (_ => null)).WithTracking(this.GetBlogTracker);
            this.getPostsFunc = (getPostsFunc ?? (_ => null)).WithTracking(this.GetPostsTracker);
        }

        public override Blog GetBlog(string blogId)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            var blog = this.getBlogFunc?.Invoke(blogId);
            return blog;
        }

        public DelegateTracker<string> GetBlogTracker { get; } = new DelegateTracker<string>();

        public override IEnumerable<Post> GetPosts(string blogId, DateTime? lastUpdatedAt)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            var posts = this.getPostsFunc?.Invoke(blogId);

            if (lastUpdatedAt > DateTime.MinValue)
            {
                posts = posts?.Where(x => x.Updated > lastUpdatedAt);
            }

            return posts;
        }

        public DelegateTracker<string> GetPostsTracker { get; } = new DelegateTracker<string>();

        public DelegateTracker<string> GetPostsSlimTracker { get; } = new DelegateTracker<string>();
    }
}
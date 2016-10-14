using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Blaven.Tests;

namespace Blaven.BlogSources.Blogger.Tests
{
    [DebuggerDisplay(
         "GetBlogPostsTracker={GetBlogTracker.Events.Count}, " + "SaveBlogMetaTracker={GetPostsTracker.Events.Count}, "
         + "SaveChangesTracker={GetPostsSlimTracker.Events.Count}")]
    public class MockBloggerApiProvider : IBloggerApiProvider
    {
        private readonly Func<string, BloggerBlogData> getBlogFunc;

        private readonly Func<string, IEnumerable<BloggerPostData>> getPostsFunc;

        public MockBloggerApiProvider(
            Func<string, BloggerBlogData> getBlogFunc = null,
            Func<string, IEnumerable<BloggerPostData>> getPostsFunc = null)
        {
            this.getBlogFunc = (getBlogFunc ?? (_ => null)).WithTracking(this.GetBlogTracker);
            this.getPostsFunc = (getPostsFunc ?? (_ => null)).WithTracking(this.GetPostsTracker);
        }

        public async Task<BloggerBlogData> GetBlog(string blogId)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            var blog = this.getBlogFunc?.Invoke(blogId);
            return await Task.FromResult(blog);
        }

        public DelegateTracker<string> GetBlogTracker { get; } = new DelegateTracker<string>();

        public async Task<IReadOnlyList<BloggerPostData>> GetPosts(string blogId, DateTime? lastUpdatedAt)
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

            return await Task.FromResult(posts.ToReadOnlyList());
        }

        public DelegateTracker<string> GetPostsTracker { get; } = new DelegateTracker<string>();

        public DelegateTracker<string> GetPostsSlimTracker { get; } = new DelegateTracker<string>();
    }
}
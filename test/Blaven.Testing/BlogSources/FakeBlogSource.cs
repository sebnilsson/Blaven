using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.BlogSources.Tests
{
    public class FakeBlogSource : IBlogSource
    {
        private readonly ICollection<BlogMeta> blogMetas;

        private readonly ICollection<BlogPost> blogPosts;

        public FakeBlogSource(IEnumerable<BlogPost> blogPosts = null, IEnumerable<BlogMeta> blogMetas = null)
        {
            this.blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
            this.blogMetas = (blogMetas ?? Enumerable.Empty<BlogMeta>()).ToList();
        }

        public event EventHandler<string> OnGetaMetaRun;

        public event EventHandler<string> OnGetBlogPostsRun;

        public Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var blogMeta = this.blogMetas.SingleOrDefault(x => x.BlogKey == blogSetting.BlogKey);

            this.OnGetaMetaRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(blogMeta);
        }

        public Task<IReadOnlyList<BlogPost>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var posts =
                this.blogPosts.Where(
                        x => x.BlogKey == blogSetting.BlogKey && (lastUpdatedAt == null || x.UpdatedAt > lastUpdatedAt))
                    .ToReadOnlyList();

            this.OnGetBlogPostsRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(posts);
        }
    }
}
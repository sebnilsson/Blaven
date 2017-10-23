using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.BlogSources.Testing
{
    public class FakeBlogSource : IBlogSource
    {
        private readonly ICollection<BlogMeta> _blogMetas;
        private readonly ICollection<BlogPost> _blogPosts;

        public FakeBlogSource(IEnumerable<BlogPost> blogPosts = null, IEnumerable<BlogMeta> blogMetas = null)
        {
            _blogPosts = (blogPosts ?? Enumerable.Empty<BlogPost>()).ToList();
            _blogMetas = (blogMetas ?? Enumerable.Empty<BlogMeta>()).ToList();
        }

        public event EventHandler<string> OnGetaMetaRun;

        public event EventHandler<string> OnGetBlogPostsRun;

        public Task<IReadOnlyList<BlogPost>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var posts = _blogPosts.Where(
                    x => x.BlogKey == blogSetting.BlogKey && (lastUpdatedAt == null || x.UpdatedAt > lastUpdatedAt))
                .ToReadOnlyList();

            OnGetBlogPostsRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(posts);
        }

        public Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            var blogMeta = _blogMetas.SingleOrDefault(x => x.BlogKey == blogSetting.BlogKey);

            OnGetaMetaRun?.Invoke(this, blogSetting.BlogKey);

            return Task.FromResult(blogMeta);
        }
    }
}
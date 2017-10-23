using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerBlogSource : IBlogSource
    {
        private readonly IBloggerApiProvider _apiProvider;

        public BloggerBlogSource(string apiKey)
            : this(new BloggerApiProvider(apiKey))
        {
        }

        internal BloggerBlogSource(IBloggerApiProvider apiProvider)
        {
            _apiProvider = apiProvider ?? throw new ArgumentNullException(nameof(apiProvider));
        }

        public async Task<IReadOnlyList<BlogPost>> GetBlogPosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));

            var posts = await _apiProvider.GetPosts(blogSetting.Id, lastUpdatedAt);
            if (posts == null)
            {
                var message =
                    $"{nameof(IBloggerApiProvider)} returned a null result from {nameof(_apiProvider.GetPosts)}"
                    + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BloggerBlogSourceException(message);
            }

            var blogPosts = posts.Where(x => x != null).Select(x => GetBlogPost(x, blogSetting)).ToReadOnlyList();
            return blogPosts;
        }

        public async Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
                throw new ArgumentNullException(nameof(blogSetting));

            var blog = await _apiProvider.GetBlog(blogSetting.Id);
            if (blog == null)
            {
                var message = $"{nameof(BloggerApiProvider)} returned a null result from {nameof(_apiProvider.GetBlog)}"
                              + $" for {nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BloggerBlogSourceException(message);
            }

            var blogMeta = BloggerDataConverter.ConvertMeta(blog);
            blogMeta.BlogKey = blogSetting.BlogKey;

            return blogMeta;
        }

        private static BlogPost GetBlogPost(BloggerPostData post, BlogSetting blogSetting)
        {
            var blogPost = BloggerDataConverter.ConvertPost(post);
            blogPost.BlogKey = blogSetting.BlogKey;

            return blogPost;
        }
    }
}
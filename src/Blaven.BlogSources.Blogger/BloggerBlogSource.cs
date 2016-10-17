using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerBlogSource : IBlogSource
    {
        private readonly IBloggerApiProvider apiProvider;

        public BloggerBlogSource(string apiKey)
            : this(GetBloggerApiProvider(apiKey))
        {
        }

        internal BloggerBlogSource(IBloggerApiProvider apiProvider)
        {
            if (apiProvider == null)
            {
                throw new ArgumentNullException(nameof(apiProvider));
            }

            this.apiProvider = apiProvider;
        }

        public async Task<BlogMeta> GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var blog = await this.apiProvider.GetBlog(blogSetting.Id);
            if (blog == null)
            {
                string message =
                    $"{nameof(BloggerApiProvider)} returned a null result from {nameof(this.apiProvider.GetBlog)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BloggerBlogSourceException(message);
            }

            var blogMeta = BloggerDataConverter.ConvertMeta(blog);
            blogMeta.BlogKey = blogSetting.BlogKey;

            return blogMeta;
        }

        public async Task<IReadOnlyList<BlogPost>> GetBlogPosts(
            BlogSetting blogSetting,
            IEnumerable<BlogPostBase> dataStoragePosts,
            DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var posts = await this.apiProvider.GetPosts(blogSetting.Id, lastUpdatedAt);
            if (posts == null)
            {
                string message =
                    $"{nameof(BloggerApiProvider)} returned a null result from {nameof(this.apiProvider.GetPosts)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BloggerBlogSourceException(message);
            }

            var blogPosts = posts.Where(x => x != null).Select(x => GetBlogPost(x, blogSetting)).ToReadOnlyList();
            return blogPosts;
        }

        private static BlogPost GetBlogPost(BloggerPostData post, BlogSetting blogSetting)
        {
            var blogPost = BloggerDataConverter.ConvertPost(post);
            blogPost.BlogKey = blogSetting.BlogKey;

            return blogPost;
        }

        private static BloggerApiProvider GetBloggerApiProvider(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var apiProvider = new BloggerApiProvider(apiKey);
            return apiProvider;
        }
    }
}
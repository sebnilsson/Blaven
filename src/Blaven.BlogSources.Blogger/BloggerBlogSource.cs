using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Blogger.v3.Data;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerBlogSource : BlogSourceBase
    {
        private readonly BloggerApiProvider apiProvider;

        public BloggerBlogSource(string apiKey)
            : this(GetBloggerApiProvider(apiKey))
        {
        }

        internal BloggerBlogSource(BloggerApiProvider apiProvider)
        {
            if (apiProvider == null)
            {
                throw new ArgumentNullException(nameof(apiProvider));
            }

            this.apiProvider = apiProvider;
        }

        public override BlogMeta GetMeta(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var blog = this.apiProvider.GetBlog(blogSetting.Id);
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

        protected override IEnumerable<BlogPost> GetSourcePosts(BlogSetting blogSetting, DateTime? lastUpdatedAt)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var posts = this.apiProvider.GetPosts(blogSetting.Id, lastUpdatedAt);
            if (posts == null)
            {
                string message =
                    $"{nameof(BloggerApiProvider)} returned a null result from {nameof(this.apiProvider.GetPosts)} for {nameof(blogSetting)}.{nameof(blogSetting.BlogKey)} '{blogSetting.BlogKey}'.";
                throw new BloggerBlogSourceException(message);
            }

            var blogPosts = posts.Where(x => x != null).Select(x => GetBlogPost(x, blogSetting));
            return blogPosts;
        }

        private static BlogPost GetBlogPost(Post post, BlogSetting blogSetting)
        {
            var blogPost = BloggerDataConverter.ConvertPost(post);
            blogPost.BlogKey = blogSetting.BlogKey;

            return blogPost;
        }

        public static BloggerBlogSource CreateFromAppSettings()
        {
            var bloggerBlogSource = BloggerBlogSourceAppSettingsHelper.CreateFromAppSettings();
            return bloggerBlogSource;
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
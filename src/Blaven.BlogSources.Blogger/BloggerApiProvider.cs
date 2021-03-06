﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiProvider : IBloggerApiProvider
    {
        public const int DefaultPostListRequestMaxResults = 500;
        private readonly BloggerApiUrlHelper _apiUrlHelper;

        public BloggerApiProvider(string apiKey)
        {
            if (apiKey == null)
                throw new ArgumentNullException(nameof(apiKey));

            _apiUrlHelper = new BloggerApiUrlHelper(apiKey);
        }

        internal int PostListRequestMaxResults { get; set; } = DefaultPostListRequestMaxResults;

        public async Task<BloggerBlogData> GetBlog(string blogId)
        {
            if (blogId == null)
                throw new ArgumentNullException(nameof(blogId));

            var blogUrl = _apiUrlHelper.GetBlogUrl(blogId);

            var blog = await GetHttpJsonContent<BloggerBlogData>(blogUrl);
            return blog;
        }

        public async Task<IReadOnlyList<BloggerPostData>> GetPosts(string blogId, DateTime? lastUpdatedAt)
        {
            if (blogId == null)
                throw new ArgumentNullException(nameof(blogId));

            var posts = new List<BloggerPostData>();

            BloggerPostsData postsData = null;

            while (true)
            {
                var url = _apiUrlHelper.GetPostsUrl(
                    blogId,
                    lastUpdatedAt,
                    postsData?.NextPageToken,
                    PostListRequestMaxResults);

                postsData = await GetHttpJsonContent<BloggerPostsData>(url);

                if (postsData?.Items == null || !postsData.Items.Any())
                    break;

                posts.AddRange(postsData.Items);

                if (string.IsNullOrWhiteSpace(postsData?.NextPageToken))
                    break;
            }

            return posts.ToReadOnlyList();
        }

        private static async Task<T> GetHttpJsonContent<T>(string url)
        {
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var result = JsonConvert.DeserializeObject<T>(content);
                return result;
            }
        }
    }
}
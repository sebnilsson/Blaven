﻿using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Google.Apis.Services;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerApiProvider
    {
        public const int DefaultPostListRequestMaxResults = 500;

        private readonly BloggerService bloggerService;

        private readonly int postListRequestMaxResults;

        public BloggerApiProvider(BloggerService bloggerService)
            : this(bloggerService, DefaultPostListRequestMaxResults)
        {
        }

        internal BloggerApiProvider(string apiKey, int postListRequestMaxResults = DefaultPostListRequestMaxResults)
            : this(GetBloggerService(apiKey), postListRequestMaxResults)
        {
        }

        internal BloggerApiProvider(BloggerService bloggerService, int postListRequestMaxResults)
        {
            if (bloggerService == null)
            {
                throw new ArgumentNullException(nameof(bloggerService));
            }
            if (postListRequestMaxResults < 1)
            {
                string message = $"Parameter '{postListRequestMaxResults}' must be a postive number.";
                throw new ArgumentOutOfRangeException(nameof(postListRequestMaxResults), message);
            }

            this.bloggerService = bloggerService;
            this.postListRequestMaxResults = postListRequestMaxResults;
        }

        protected BloggerApiProvider()
        {
        }

        public virtual Blog GetBlog(string blogId)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            var request = this.bloggerService.Blogs.Get(blogId);

            var blog = request.Execute();
            return blog;
        }

        public virtual IEnumerable<Post> GetPosts(string blogId)
        {
            if (blogId == null)
            {
                throw new ArgumentNullException(nameof(blogId));
            }

            var postListRequest = this.GetPostListRequest(blogId);

            var posts = GetPostListRequestAllPosts(postListRequest);
            return posts;
        }

        //public virtual IEnumerable<Post> GetPostsSlim(string blogId)
        //{
        //    if (blogId == null)
        //    {
        //        throw new ArgumentNullException(nameof(blogId));
        //    }

        //    var postListRequest = this.GetPostListRequestSlim(blogId);
        //    var posts = GetPostListRequestAllPosts(postListRequest);
        //    return posts;
        //}

        private PostsResource.ListRequest GetPostListRequest(string blogId)
        {
            var postListRequest = this.bloggerService.Posts.List(blogId);
            postListRequest.Fields = "nextPageToken,items(id,published,updated,url,title,content,author,labels)";
            postListRequest.FetchImages = false;
            postListRequest.PrettyPrint = false;
            postListRequest.MaxResults = this.postListRequestMaxResults;
            //postListRequest.OrderBy = PostsResource.ListRequest.OrderByEnum.Updated;

            return postListRequest;
        }

        //private PostsResource.ListRequest GetPostListRequestSlim(string blogId)
        //{
        //    var postListRequest = this.GetPostListRequest(blogId);
        //    postListRequest.Fields = "items(id,updated)";

        //    return postListRequest;
        //}

        private static IEnumerable<Post> GetPostListRequestAllPosts(PostsResource.ListRequest postListRequest)
        {
            var posts = postListRequest.Execute();

            foreach (var post in posts.Items ?? Enumerable.Empty<Post>())
            {
                yield return post;
            }

            while (!string.IsNullOrWhiteSpace(posts.NextPageToken))
            {
                postListRequest.PageToken = posts.NextPageToken;

                posts = postListRequest.Execute();

                foreach (var post in posts.Items ?? Enumerable.Empty<Post>())
                {
                    yield return post;
                }
            }
        }

        internal static BloggerService GetBloggerService(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var initializer = new BaseClientService.Initializer { ApiKey = apiKey, ApplicationName = "Blaven" };

            var service = new BloggerService(initializer);
            return service;
        }
    }
}
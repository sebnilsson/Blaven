using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Google.Apis.Requests;

namespace Blaven.BlogSources.Blogger
{
    internal class BloggerApiHelper
    {
        private const int PostRequestMaxResults = 500;

        //public const string BloggerFeedUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        private readonly BloggerService bloggerService;

        public BloggerApiHelper(BloggerService bloggerService)
        {
            if (bloggerService == null)
            {
                throw new ArgumentNullException(nameof(bloggerService));
            }
            
            this.bloggerService = bloggerService;
        }

        public virtual BlogMeta GetMeta(string blogKey)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var request = this.bloggerService.Blogs.Get(blogKey);

            var blog = request.Execute();

            var blogMeta = new BlogMeta
                               {
                                   BlogKey = blogKey,
                                   Description = blog.Description,
                                   Name = blog.Name,
                                   Published = blog.Published,
                                   SourceId = blog.Id,
                                   Url = blog.Url,
                                   Updated = blog.Updated
                               };
            return blogMeta;
        }

        public virtual BlogSourceChangeSet GetChanges(string blogKey, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            if (blogKey == null)
            {
                throw new ArgumentNullException(nameof(blogKey));
            }

            var changes = new BlogSourceChangeSet();


            var postsRequest = this.bloggerService.Posts.List(blogKey);
            postsRequest.MaxResults = PostRequestMaxResults;
            postsRequest.OrderBy = PostsResource.ListRequest.OrderByEnum.Updated;
            // TODO: Slim request
            // postsRequest.Fields = "items(id,updated)"; // "etag,items(id,published,updated,title)"

            var posts = GetAllPosts(postsRequest);
            
            throw new NotImplementedException();
        }

        private static IEnumerable<Post> GetAllPosts(PostsResource.ListRequest postsRequest)
        {
            var posts = postsRequest.Execute();
            foreach (var post in posts.Items)
            {
                yield return post;
            }

            while (!string.IsNullOrWhiteSpace(posts.NextPageToken))
            {
                postsRequest.PageToken = posts.NextPageToken;

                var nextPagePosts = postsRequest.Execute();
                
            }
        }

        //public PostList GetBloggerDocument(BlavenBlogSetting setting, PostsResource.ListRequest request)
        //{
        //    var posts = GetPosts(request, setting);
        //    return posts;
        //}

        //public PostList GetModifiedPostsContent(DataSourceRefreshContext refreshInfo)
        //{
        //    var service = GetService(refreshInfo.BlogSetting);

        //    var request = GetRequest(service, refreshInfo.BlogSetting);
        //    request.EndDate = refreshInfo.LastRefresh ?? DateTime.MinValue;

        //    return GetBloggerDocument(refreshInfo.BlogSetting, request);
        //}

        //public IEnumerable<ulong> GetAllBloggerIds(BlavenBlogSetting setting)
        //{
        //    var service = GetService(setting);

        //    var request = GetRequest(service, setting);

        //    var posts = GetPosts(request, setting);

        //    return posts.Items.Select(x => ulong.Parse(x.Id)).ToList();
        //}

        //private PostsResource.ListRequest GetRequest(BloggerService service, BlavenBlogSetting setting)
        //{
        //    var request = service.Posts.List(setting.DataSourceId);
        //    request.MaxResults = 500;
        //    request.OrderBy = PostsResource.ListRequest.OrderByEnum.Updated;

        //    return request;
        //}

        //private PostList GetPosts(IClientServiceRequest<PostList> request, BlavenBlogSetting setting)
        //{
        //    try
        //    {
        //        var posts = request.Execute();
        //        return posts;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new BloggerApiHelperException(setting, ex);
        //    }
        //}
    }
}
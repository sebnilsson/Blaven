using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Google.Apis.Requests;
using Google.Apis.Services;

namespace Blaven.Sources.Blogger
{
    internal static class BloggerApiHelper
    {
        public const string BloggerFeedUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        public static Blog GetBlogInfo(DataSourceRefreshContext refreshInfo)
        {
            var service = GetService(refreshInfo.BlogSetting);

            var request = service.Blogs.Get(refreshInfo.BlogSetting.DataSourceId);

            var blogInfo = request.Execute();
            return blogInfo;
        }

        public static PostList GetBloggerDocument(BlavenBlogSetting setting, PostsResource.ListRequest request)
        {
            var posts = GetPosts(request, setting);
            return posts;
        }

        public static PostList GetModifiedPostsContent(DataSourceRefreshContext refreshInfo)
        {
            var service = GetService(refreshInfo.BlogSetting);

            var request = GetRequest(service, refreshInfo.BlogSetting);
            request.EndDate = refreshInfo.LastRefresh ?? DateTime.MinValue;

            return GetBloggerDocument(refreshInfo.BlogSetting, request);
        }

        public static IEnumerable<ulong> GetAllBloggerIds(BlavenBlogSetting setting)
        {
            var service = GetService(setting);

            var request = GetRequest(service, setting);

            var posts = GetPosts(request, setting);

            return posts.Items.Select(x => ulong.Parse(x.Id)).ToList();
        }

        private static BloggerService GetService(BlavenBlogSetting setting)
        {
            var initializer = new BaseClientService.Initializer
                                  {
                                      ApiKey = setting.Password,
                                      ApplicationName = "Blaven"
                                  };
            var service = new BloggerService(initializer);

            return service;
        }

        private static PostsResource.ListRequest GetRequest(BloggerService service, BlavenBlogSetting setting)
        {
            var request = service.Posts.List(setting.DataSourceId);
            request.MaxResults = 500;
            request.OrderBy = PostsResource.ListRequest.OrderByEnum.Updated;

            return request;
        }

        private static PostList GetPosts(IClientServiceRequest<PostList> request, BlavenBlogSetting setting)
        {
            try
            {
                var posts = request.Execute();
                return posts;
            }
            catch (Exception ex)
            {
                throw new BloggerApiHelperException(setting, ex);
            }
        }
    }
}
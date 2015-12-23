using System;
using System.Collections.Generic;
using System.Linq;

using Google.Apis;
using Google.Apis.Blogger.v3;
using Google.Apis.Blogger.v3.Data;
using Xunit;

namespace Blaven.Sources.Blogger.Tests.Integrations
{
    public class Test
    {
        [Fact]
        public void Testing()
        {
            var service = TestBloggerServiceHelper.Get();
            
            var request = GetBaseRequest(service);
            request.OrderBy = PostsResource.ListRequest.OrderByEnum.Updated;
            request.Fields = "nextPageToken,items(id,published,updated,etag,title)";
            request.EndDate = DateTime.Today;
            request.StartDate = new DateTime(2012, 6, 26, 1, 0, 0, DateTimeKind.Utc);
            request.ETagAction = ETagAction.IfNoneMatch;
            request.FetchBodies = false;
            request.FetchImages = false;

            var postList = request.Execute();
            var posts = (postList.Items != null) ? postList.Items.ToList() : new List<Post>(0);

            var updatedFirst = posts.FirstOrDefault();
            var updatedFirstPublished =
                (((updatedFirst != null) ? updatedFirst.Published : null) ?? DateTime.MinValue).ToUniversalTime();
            var updatedFirstUpdated =
                (((updatedFirst != null) ? updatedFirst.Updated : null) ?? DateTime.MinValue).ToUniversalTime();

            Console.WriteLine(posts.Count);

            //var allPosts = GetAllPosts(service);

            //Console.WriteLine(allPosts.Count);
        }

        private static List<Post> GetAllPosts(BloggerService service)
        {
            var request = GetBaseRequest(service);
            request.Fields = "nextPageToken,items(id,etag,published)";

            var posts = new List<Post>();

            string nextPageToken;
            do
            {
                var list = request.Execute();
                var items = list.Items ?? new List<Post>(0);

                posts.AddRange(items);

                request.PageToken = nextPageToken = list.NextPageToken;
            }
            while (!string.IsNullOrWhiteSpace(nextPageToken));

            return posts;
        }

        private static PostsResource.ListRequest GetBaseRequest(BloggerService service)
        {
            var request = service.Posts.List("6269875262163043800"); // 10861780
            request.MaxResults = 500;

            return request;
        }
    }
}
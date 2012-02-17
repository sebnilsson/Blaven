using System;
using System.Collections.Generic;
using System.Linq;

using Google.GData.Blogger;
using Google.GData.Client;

namespace BloggerViewController {
    public class BlogService {
        private string _blogId;
        private string _username;
        private string _password;

        public BlogService(string blogId, string username, string password) {
            _blogId = blogId;
            _username = username;
            _password = password;
        }

        public BlogData GetBlogData() {
            return GetBlogDataMultiple(null, null);
        }

        public BlogData GetBlogData(int pageSize, int pageIndex) {
            return GetBlogDataMultiple(pageSize, pageIndex);
        }

        public BlogData GetBlogData(string entryId) {
            return GetBlogDataSingle(entryId);
        }

        private BlogData GetBlogDataSingle(string entryId) {
            // TODO: Get correct URI
            var queryUri = new Uri(string.Format("https://www.blogger.com/feeds/{0}/posts/default?entryId={1}", _blogId, entryId));
            return GetBlogDataFromService(1, 0, queryUri);
        }

        private BlogData GetBlogDataMultiple(int? pageSize, int? pageIndex) {
            var queryUri = new Uri(string.Format("https://www.blogger.com/feeds/{0}/posts/default", _blogId));
            return GetBlogDataFromService(pageSize, pageIndex, queryUri);
        }

        private BloggerService GetBloggerService() {
            var service = new BloggerService("BloggerViewController");
            service.Credentials = new GDataCredentials(_username, _password);

            // For proper authentication for Google Apps users
            SetAuthForGoogleAppsUsers(service);

            return service;
        }

        private BlogData GetBlogDataFromService(int? pageSize, int? pageIndex, Uri queryUri) {
            var service = GetBloggerService();

            var query = new BloggerQuery {
                Uri = queryUri,
            };
            if(pageSize.HasValue && pageIndex.HasValue) {
                int startIndex = pageSize.Value * pageIndex.Value;

                query.NumberToRetrieve = pageSize.Value;
                query.StartIndex = startIndex;
            }

            var feed = service.Query(query);

            bool hasNextItems = !string.IsNullOrWhiteSpace(feed.NextChunk);

            var blogResult = new BlogData(hasNextItems, pageSize, pageIndex) {
                Categories = (feed.Categories != null)
                    ? feed.Categories.Select(cat => cat.Term) : Enumerable.Empty<string>(),
                Description = feed.Subtitle.Text,
                Title = feed.Title.Text,
                Updated = feed.Updated,
            };

            if(pageSize.GetValueOrDefault(0) > 0) {
                blogResult.Posts = GetBlogPostDetails(service, query, feed);
            }

            return blogResult;
        }

        private IEnumerable<BlogPostDetail> GetBlogPostDetails(Service service, BloggerQuery query, AtomFeed feed) {
            foreach(var entry in feed.Entries) {
                yield return new BlogPostDetail(entry);
            }

            var next = feed.NextChunk;
            while(!string.IsNullOrWhiteSpace(next)) {
                query.Uri = new Uri(next);
                feed = service.Query(query);
                foreach(var entry in feed.Entries) {
                    yield return new BlogPostDetail(entry);
                }

                next = feed.NextChunk;
            }
        }

        public IEnumerable<BlogPost> GetAllPosts() {
            var service = GetBloggerService();

            var query = new BloggerQuery {
                Uri = new Uri(string.Format("https://www.blogger.com/feeds/{0}/posts/default", _blogId)),
            };
            query.ExtraParameters = "?fields=entry(id,title,published,uri)";

            var feed = service.Query(query);

            return GetBlogPosts(service, query, feed);
        }

        private IEnumerable<BlogPost> GetBlogPosts(Service service, BloggerQuery query, AtomFeed feed) {
            foreach(var entry in feed.Entries) {
                yield return new BlogPost(entry);
            }

            var next = feed.NextChunk;
            while(!string.IsNullOrWhiteSpace(next)) {
                query.Uri = new Uri(next);
                feed = service.Query(query);
                foreach(var entry in feed.Entries) {
                    yield return new BlogPost(entry);
                }

                next = feed.NextChunk;
            }
        }

        private void SetAuthForGoogleAppsUsers(BloggerService service) {
            GDataGAuthRequestFactory factory = service.RequestFactory as GDataGAuthRequestFactory;
            if(factory == null) {
                return;
            }

            factory.AccountType = "GOOGLE";
        }
    }
}

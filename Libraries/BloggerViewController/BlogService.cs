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

        public BlogResult GetBlog(DateTime? minPublicationDate = null, bool includePosts = true) {
            var service = new BloggerService("BloggerViewController");
            service.Credentials = new GDataCredentials(_username, _password);
            
            // For proper authentication for Google Apps users
            SetAuthForGoogleAppsUsers(service);

            var query = new BloggerQuery();
            query.Uri = new Uri(string.Format("https://www.blogger.com/feeds/{0}/posts/default", _blogId));
            if(!includePosts) {
                query.NumberToRetrieve = 0;
            }
            
            if(minPublicationDate.HasValue) {
                query.MinPublication = minPublicationDate.Value;
            }
            
            var feed = service.Query(query);

            var blogResult = new BlogResult {
                Categories = (feed.Categories != null)
                    ? feed.Categories.Select(cat => cat.Term) : Enumerable.Empty<string>(),
                Description = feed.Subtitle.Text,
                Title = feed.Title.Text,
                Updated = feed.Updated,
            };

            if(includePosts) {
                blogResult.Posts = GetBlogPosts(service, query, feed);
            }

            return blogResult;
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

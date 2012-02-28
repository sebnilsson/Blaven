using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Google.GData.Blogger;
using Google.GData.Client;

namespace BloggerViewController {
    internal class BloggerHelper {
        private string _blogId;
        private string _username;
        private string _password;

        public static string BloggerPostsUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        public BloggerHelper(string blogId, string username, string password) {
            _blogId = blogId;
            _username = username;
            _password = password;
        }

        public XDocument GetBloggerDocument(DateTime? ifModifiedSince = null) {
            var service = GetBloggerService();

            var query = new BloggerQuery {
                Uri = new Uri(string.Format(BloggerPostsUriFormat, _blogId)),
                NumberToRetrieve = int.MaxValue,
            };

            if(ifModifiedSince.HasValue) {
                query.ModifiedSince = ifModifiedSince.Value;
            }

            var feed = service.Query(query);

            using(var stream = new System.IO.MemoryStream()) {
                feed.SaveToXml(stream);
                stream.Position = 0;

                /*using(var reader = new System.IO.StreamReader(stream)) {
                    string content = reader.ReadToEnd();
                }*/

                var document = XDocument.Load(stream);
                return document;
            }
        }

        public static BlogData ParseBlogData(XDocument document) {
            var feed = document.Root;
            var ns = document.Root.Name.Namespace;

            var blogEntries = feed.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in blogEntries
                               select new BlogPost {
                                   ID = ParseId(entry.Element(ns + "id").Value),
                                   Categories = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                                   Content = entry.Element(ns + "content").Value,
                                   EditUri = entry.Elements(ns + "link").First(el => el.Attribute("rel").Value == "edit").Attribute("href").Value,
                                   Published = ParseDate(entry.Element(ns + "published").Value),
                                   Title = entry.Element(ns + "title").Value,
                                   Updated = ParseDate(entry.Element(ns + "updated").Value),
                                   Uri = entry.Elements(ns + "link").First(el => el.Attribute("rel").Value == "self").Attribute("href").Value,
                               };

            var categories = new Dictionary<string, int>();
            foreach(var post in posts) {
                foreach(var category in post.Categories) {
                    if(!categories.ContainsKey(category)) {
                        categories.Add(category, 0);
                    }
                    categories[category] = (categories[category] + 1);
                }
            }

            var postDates = new Dictionary<DateTime, int>();
            foreach(var post in posts) {
                DateTime key = new DateTime(post.Published.Year, post.Published.Month, 1);
                if(!postDates.ContainsKey(key)) {
                    postDates.Add(key, 0);
                }
                postDates[key] = (postDates[key] + 1);
            }

            var blogInfo = new BlogInfo(categories, postDates) {
                Subtitle = feed.Element(ns + "subtitle").Value,
                Title = feed.Element(ns + "title").Value,
                Updated = ParseDate(feed.Element(ns + "updated").Value),
            };

            var blogData = new BlogData(blogInfo, posts);
            return blogData;
        }

        /*public BlogSelection GetBlogSection(XDocument document, BlogInfo blogInfo, int pageIndex, int? pageSize) {
            return GetBlogSection(document, blogInfo.AllPosts, pageIndex, pageSize);
        }

        public BlogSelection GetBlogSection(XDocument document, IEnumerable<BlogPost> allPosts, int pageIndex, int? pageSize) {
            int take = pageSize.GetValueOrDefault(BlogConfiguration.PageSize);
            int skip = (pageIndex * take);
            var blogEntries = document.Root.Elements("entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value)).Skip(skip).Take(take);

            var blogPosts = ParseBlogPostDetails(blogEntries);

            return new BlogSelection(allPosts, blogPosts, pageIndex, pageSize);
        }*/

        /*public BlogPostDetail GetBlogPost(XDocument document, IEnumerable<BlogPost> allPosts, string blogId) {
            var blogEntries = document.Root.Elements("entry").Where(entry => entry.Value.EndsWith(blogId));

            var blogPosts = ParseBlogPostDetails(blogEntries);

            return blogPosts.FirstOrDefault();
        }

        private IEnumerable<BlogPostDetail> ParseBlogPostDetails(IEnumerable<XElement> elements) {
            var blogPosts = from entry in elements
                            select new BlogPostDetail {
                                ID = ParseId(entry.Element("id").Value),
                                Categories = entry.Elements("category").Select(cat => cat.Attribute("term").Value),
                                Published = ParseDate(entry.Element("published").Value),
                                Title = entry.Element("title").Value,
                                Uri = entry.Elements("link").First(el => el.Attribute("rel").Value == "self").Attribute("href").Value,

                                Content = entry.Element("content").Value,
                                EditUri = entry.Elements("link").First(el => el.Attribute("rel").Value == "edit").Attribute("href").Value,
                                Updated = ParseDate(entry.Element("updated").Value),
                            };

            return blogPosts;
        }*/

        private BloggerService GetBloggerService() {
            var service = new BloggerService("BloggerViewController");
            service.Credentials = new GDataCredentials(_username, _password);

            // For proper authentication for Google Apps users
            SetAuthForGoogleAppsUsers(service);

            return service;
        }

        private void SetAuthForGoogleAppsUsers(BloggerService service) {
            GDataGAuthRequestFactory factory = service.RequestFactory as GDataGAuthRequestFactory;
            if(factory == null) {
                return;
            }

            factory.AccountType = "GOOGLE";
        }

        internal static string ParseId(string val) {
            string findValue = ".post-";
            int index = val.IndexOf(findValue) + findValue.Length;

            return val.Substring(index);
        }

        internal static DateTime ParseDate(string val) {
            var split = val.Split(new[] { 'T' }, StringSplitOptions.RemoveEmptyEntries);
            var timeString = split[1].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var date = DateTime.Parse(split[0]);
            var time = DateTime.Parse(timeString);

            var dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            return dateTime;
        }
    }
}

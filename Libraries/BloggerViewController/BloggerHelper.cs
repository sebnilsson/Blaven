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

            BloggerFeed feed = null;
            try {
                feed = service.Query(query);
            }
            catch(GDataNotModifiedException) {
                return null;
            }

            using(var stream = new System.IO.MemoryStream()) {
                feed.SaveToXml(stream);
                stream.Position = 0;

                var document = XDocument.Load(stream);
                return document;
            }
        }

        public static BlogData ParseBlogData(XDocument document) {
            var feed = document.Root;
            var ns = document.Root.Name.Namespace;

            var blogEntries = feed.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in blogEntries
                        select ParseEntry(ns, entry);

            var categories = new Dictionary<string, int>();
            foreach(var post in posts) {
                foreach(var category in post.Categories) {
                    if(!categories.ContainsKey(category)) {
                        categories.Add(category, 0);
                    }
                    categories[category] = (categories[category] + 1);
                }
            }

            var friendlyPermaLinks = posts.Select(post => post.FriendlyPermaLink);

            var postDates = new Dictionary<DateTime, int>();
            foreach(var post in posts) {
                DateTime key = new DateTime(post.Published.Year, post.Published.Month, 1);
                if(!postDates.ContainsKey(key)) {
                    postDates.Add(key, 0);
                }
                postDates[key] = (postDates[key] + 1);
            }
                        
            var blogInfo = new BlogInfo(categories, friendlyPermaLinks, postDates) {
                Subtitle = feed.Element(ns + "subtitle").Value,
                Title = feed.Element(ns + "title").Value,
                Updated = ParseDate(feed.Element(ns + "updated").Value),
            };

            var blogData = new BlogData(blogInfo, posts);
            return blogData;
        }

        private static BlogPost ParseEntry(XNamespace ns, XElement entry) {
            var alternateLink = entry.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            var post = new BlogPost {
                ID = ParseId(entry.Element(ns + "id").Value),
                Categories = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                Content = entry.Element(ns + "content").Value,
                FriendlyPermaLink = GetRelativeUrl(alternateLink == null ? string.Empty : alternateLink.Attribute("href").Value),
                Published = ParseDate(entry.Element(ns + "published").Value),
                Title = entry.Element(ns + "title").Value,
                Updated = ParseDate(entry.Element(ns + "updated").Value),
            };
            return post;
        }

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

        internal static string GetRelativeUrl(string fullUrl) {
            if(string.IsNullOrWhiteSpace(fullUrl) || !Uri.IsWellFormedUriString(fullUrl, UriKind.RelativeOrAbsolute)) {
                return string.Empty;
            }
            var uri = new Uri(fullUrl);

            var relative = uri.LocalPath;
            var index = relative.LastIndexOf(".html");
            relative = relative.Substring(0, index);
            return relative;
        }
    }
}

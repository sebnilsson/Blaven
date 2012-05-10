﻿using System;
using System.Linq;
using System.Xml.Linq;

using Google.GData.Blogger;
using Google.GData.Client;

namespace Blaven.Blogger {
    internal class BloggerHelper {
        public const string BloggerFeedUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        public XDocument GetBloggerDocument(BloggerSetting setting, DateTime? ifModifiedSince = null) {
            string uri = (!string.IsNullOrWhiteSpace(setting.BloggerUri)) ? setting.BloggerUri
                : string.Format(BloggerFeedUriFormat, setting.BlogId);
                        
            var query = new BloggerQuery(uri) {
                NumberToRetrieve = int.MaxValue,
            };

            if(ifModifiedSince.HasValue) {
                query.ModifiedSince = ifModifiedSince.Value;
            }

            return GetBloggerDocument(setting, query);
        }

        public XDocument GetBloggerDocument(BloggerSetting setting, BloggerQuery query) {
            var service = GetBloggerService(setting.Username, setting.Password, query.Uri.IsFile);
            
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

                return XDocument.Load(stream);
            }
        }

        public static BlogData ParseBlogData(string blogKey, XDocument document) {
            var feed = document.Root;
            var ns = document.Root.Name.Namespace;

            var blogEntries = feed.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in blogEntries
                        where entry.Element(ns + "title") != null
                        && entry.Elements(ns + "link").Any(el => el.Attribute("rel").Value == "alternate")
                        select ParseEntry(blogKey, ns, entry);
            
            var subtitle = feed.Element(ns + "subtitle");
            var altLink = feed.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            var blogInfo = new BlogInfo {
                BlogKey = blogKey,                 
                Subtitle = (subtitle != null) ? subtitle.Value : string.Empty,
                Title = feed.Element(ns + "title").Value,
                Updated = ParseDate(feed.Element(ns + "updated").Value),
                Url = (altLink != null) ? ((altLink.Attribute("href") != null) ? altLink.Attribute("href").Value : string.Empty) : string.Empty,
            };

            return new BlogData { Info = blogInfo, Posts = posts, };
        }

        private static BlogPost ParseEntry(string blogKey, XNamespace ns, XElement entry) {
            var alternateLink = entry.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");
            
            string permaLinkFull = alternateLink == null ? string.Empty : alternateLink.Attribute("href").Value;

            var post = new BlogPost(blogKey) {
                ID = ParseId(entry.Element(ns + "id").Value),
                Tags = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                Content = entry.Element(ns + "content").Value,
                PermaLinkAbsolute = permaLinkFull,
                PermaLinkRelative = GetRelativeUrl(permaLinkFull),
                Published = ParseDate(entry.Element(ns + "published").Value),
                Title = entry.Element(ns + "title").Value,
                Updated = ParseDate(entry.Element(ns + "updated").Value),
            };

            var authorNode = entry.Element(ns + "author");
            if(authorNode != null) {
                post.Author.Name = authorNode.Element(ns + "name").Value;

                var gdNs = XNamespace.Get("http://schemas.google.com/g/2005");
                post.Author.ImageUrl = authorNode.Element(gdNs + "image").Attribute("src").Value;
            }

            return post;
        }

        private BloggerService GetBloggerService(string username, string password, bool isFile = false) {
            var service = new BloggerService("Blaven");
            if(!string.IsNullOrWhiteSpace(username)) {
                service.Credentials = new GDataCredentials(username, password);

                // For proper authentication for Google Apps users
                SetAuthForGoogleAppsUsers(service);
            }

            if(isFile) {
                service.RequestFactory = new Blaven.Data.LocalGDataRequestFactory();   
            }

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

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
        }

        internal static string GetRelativeUrl(string fullUrl) {
            if(string.IsNullOrWhiteSpace(fullUrl) || !Uri.IsWellFormedUriString(fullUrl, UriKind.RelativeOrAbsolute)) {
                return string.Empty;
            }
            var uri = new Uri(fullUrl);

            var relative = uri.LocalPath;
            var index = relative.LastIndexOf(".html");
            if(index < 0) {
                index = relative.LastIndexOf(".asp");
            }

            return relative.Substring(0, index);
        }
    }
}

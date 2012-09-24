using System;
using System.Linq;
using System.Xml.Linq;

using Blaven.Transformers;

namespace Blaven.Blogger {
    internal static class BloggerParser {
        public static BlogData ParseBlogData(BloggerSetting bloggerSetting, string bloggerContent) {
            try {
                return ParseBlogDataImpl(bloggerSetting, bloggerContent);
            }
            catch(Exception ex) {
                throw new BloggerParsingException(bloggerSetting.BlogKey, bloggerContent, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(BloggerSetting bloggerSetting, string bloggerContent) {
            var bloggerDocument = XDocument.Parse(bloggerContent);

            var feed = bloggerDocument.Root;
            var ns = bloggerDocument.Root.Name.Namespace;

            var blogEntries = feed.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in blogEntries
                        where entry.Element(ns + "title") != null
                        && entry.Elements(ns + "link").Any(el => el.Attribute("rel").Value == "alternate")
                        select ParseEntry(bloggerSetting, ns, entry);

            var subtitle = feed.Element(ns + "subtitle");
            var altLink = feed.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            var blogInfo = new BlogInfo {
                BlogKey = bloggerSetting.BlogKey,
                Subtitle = (subtitle != null) ? subtitle.Value : string.Empty,
                Title = feed.Element(ns + "title").Value,
                Updated = ParseDate(feed.Element(ns + "updated").Value),
                Url = (altLink != null) ? ((altLink.Attribute("href") != null) ? altLink.Attribute("href").Value : string.Empty) : string.Empty,
            };

            return new BlogData { Info = blogInfo, Posts = posts, };
        }

        private static BlogPost ParseEntry(BloggerSetting bloggerSetting, XNamespace ns, XElement entry) {
            var alternateLink = entry.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            string permaLinkFull = alternateLink == null ? string.Empty : alternateLink.Attribute("href").Value;

            long id = ParseId(entry.Element(ns + "id").Value);

            var post = new BlogPost(bloggerSetting.BlogKey, id) {
                Tags = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                Content = entry.Element(ns + "content").Value ?? string.Empty,
                PermaLinkAbsolute = GetAbsoluteUrl(permaLinkFull, bloggerSetting.BaseUrl),
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

            ApplyTransformers(post);

            return post;
        }

        private static void ApplyTransformers(BlogPost blogPost) {
            var transformers = TransformersService.Instance.RegisteredBlogPostTransformers;
            foreach(var transformer in transformers) {
                transformer.Transform(blogPost);
            }
        }

        private static long ParseId(string val) {
            string findValue = ".post-";
            int index = val.IndexOf(findValue) + findValue.Length;

            string text = val.Substring(index);
            return long.Parse(text);
        }

        private static DateTime ParseDate(string val) {
            var split = val.Split(new[] { 'T' }, StringSplitOptions.RemoveEmptyEntries);
            var timeString = split[1].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var date = DateTime.Parse(split[0]);
            var time = DateTime.Parse(timeString);

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc);
        }

        private static string GetAbsoluteUrl(string fullUrl, string baseUrl) {
            if(string.IsNullOrWhiteSpace(baseUrl) || !Uri.IsWellFormedUriString(baseUrl, UriKind.Absolute)) {
                return fullUrl;
            }

            string relative = GetRelativeUrl(fullUrl);
            var baseUri = new Uri(baseUrl);

            Uri result;
            if(Uri.TryCreate(baseUri, relative, out result)) {
                return result.ToString();
            }

            return fullUrl;
        }
        
        private static string GetRelativeUrl(string fullUrl) {
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

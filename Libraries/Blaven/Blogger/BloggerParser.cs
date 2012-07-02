using System;
using System.Linq;
using System.Xml.Linq;

namespace Blaven.Blogger {
    internal static class BloggerParser {
        public static BlogData ParseBlogData(string blogKey, XDocument bloggerDocument, bool reformatParagraphs) {
            try {
                return ParseBlogDataImpl(blogKey, bloggerDocument, reformatParagraphs);
            }
            catch(Exception ex) {
                throw new BloggerParsingException(blogKey, bloggerDocument, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(string blogKey, XDocument bloggerDocument, bool reformatParagraphs) {
            var feed = bloggerDocument.Root;
            var ns = bloggerDocument.Root.Name.Namespace;

            var blogEntries = feed.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in blogEntries
                        where entry.Element(ns + "title") != null
                        && entry.Elements(ns + "link").Any(el => el.Attribute("rel").Value == "alternate")
                        select ParseEntry(blogKey, ns, entry, reformatParagraphs);

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

        private static BlogPost ParseEntry(string blogKey, XNamespace ns, XElement entry, bool reformatParagraphs) {
            var alternateLink = entry.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            string permaLinkFull = alternateLink == null ? string.Empty : alternateLink.Attribute("href").Value;

            string content = GetContent(entry.Element(ns + "content").Value, reformatParagraphs);
            long id = ParseId(entry.Element(ns + "id").Value);
            var post = new BlogPost(blogKey, id) {
                Tags = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                Content = content,
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

        private static string GetContent(string content, bool reformatParagraphs) {
            if(!reformatParagraphs) {
                return content;
            }

            string reformatted = content;

            string tooLongNewLine = string.Format("<br />\n<br />\n<br/>\n", Environment.NewLine);
            string correctNewLine = string.Format("<br />\n<br />\n", Environment.NewLine);
            while(reformatted.Contains(tooLongNewLine)) {
                reformatted = reformatted.Replace(tooLongNewLine, correctNewLine);
            }

            reformatted = reformatted.Replace(correctNewLine, "</p>\n<p>");
            
            if(reformatted != content) {
                reformatted = "<p>" + reformatted + "</p>";
            }

            return reformatted;
        }

        internal static long ParseId(string val) {
            string findValue = ".post-";
            int index = val.IndexOf(findValue) + findValue.Length;

            string text = val.Substring(index);
            return long.Parse(text);
        }

        internal static DateTime ParseDate(string val) {
            var split = val.Split(new[] { 'T' }, StringSplitOptions.RemoveEmptyEntries);
            var timeString = split[1].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0];
            var date = DateTime.Parse(split[0]);
            var time = DateTime.Parse(timeString);

            return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, DateTimeKind.Utc);
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

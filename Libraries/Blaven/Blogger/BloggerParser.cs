using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

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
            string content = ParseContent(entry, ns);

            var post = new BlogPost(bloggerSetting.BlogKey, id) {
                Tags = entry.Elements(ns + "category").Select(cat => cat.Attribute("term").Value),
                Content = content,
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

            return post;
        }

        private static long ParseId(string val) {
            string findValue = ".post-";
            int index = val.IndexOf(findValue) + findValue.Length;

            string text = val.Substring(index);
            return long.Parse(text);
        }

        private static string ParseContent(XElement entry, XNamespace ns) {
            string content = entry.Element(ns + "content").Value ?? string.Empty;

            if(!content.Contains("</pre>")) {
                return content;
            }

            string parsedContent = content;

            int openTagStartIndex = parsedContent.IndexOf("<pre");
            int openTagEndIndex = (openTagStartIndex >= 0) ? parsedContent.IndexOf(">", openTagStartIndex) : -1;
            int closeTagStartIndex = (openTagStartIndex >= 0) ? parsedContent.IndexOf("</pre>", openTagEndIndex) : -1;

            while(openTagStartIndex >= 0 && closeTagStartIndex >= 0) {
                if(parsedContent.Substring(openTagEndIndex + 1, 5) == "<code") {
                    openTagStartIndex = openTagEndIndex + 1;
                    openTagEndIndex = parsedContent.IndexOf(">", openTagStartIndex);
                    closeTagStartIndex = parsedContent.IndexOf("</code>", openTagEndIndex);
                }

                string preContent = content.Substring(openTagEndIndex + 1, closeTagStartIndex - openTagEndIndex - 1);

                string encodedPre = HttpUtility.HtmlEncode(preContent);

                parsedContent = parsedContent.Remove(openTagEndIndex + 1, closeTagStartIndex - openTagEndIndex - 1);
                parsedContent = parsedContent.Insert(openTagEndIndex + 1, encodedPre);

                openTagStartIndex = content.IndexOf("<pre", closeTagStartIndex);
                if(openTagStartIndex < 0) {
                    break;
                }
                openTagEndIndex = content.IndexOf(">", openTagStartIndex);
                closeTagStartIndex = content.IndexOf("</pre>", openTagEndIndex);
            }

            return parsedContent;
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

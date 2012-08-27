﻿using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Blaven.Blogger {
    internal static class BloggerParser {
        public static BlogData ParseBlogData(string blogKey, string bloggerContent) {
            try {
                return ParseBlogDataImpl(blogKey, bloggerContent);
            }
            catch(Exception ex) {
                throw new BloggerParsingException(blogKey, bloggerContent, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(string blogKey, string bloggerContent) {
            var bloggerDocument = XDocument.Parse(bloggerContent);

            var feed = bloggerDocument.Root;
            var ns = bloggerDocument.Root.Name.Namespace;

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

            long id = ParseId(entry.Element(ns + "id").Value);
            string content = ParseContent(entry, ns);

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

            int preStartIndex = content.IndexOf("<pre");
            int preEndIndex = (preStartIndex >= 0) ? content.IndexOf(">", preStartIndex) : -1;
            int preCloseIndex = (preStartIndex >= 0) ? content.IndexOf("</pre>", preEndIndex) : -1;

            while(preStartIndex >= 0 && preCloseIndex >= 0) {
                string preContent = parsedContent.Substring(preEndIndex + 1, preCloseIndex - preEndIndex);

                string encodedPre = HttpUtility.HtmlEncode(preContent);

                parsedContent = parsedContent.Remove(preEndIndex + 1, preCloseIndex - preEndIndex - 1);
                parsedContent = parsedContent.Insert(preEndIndex + 1, encodedPre);

                preStartIndex = content.IndexOf("<pre", preStartIndex + 1);
                if(preStartIndex < 0) {
                    break;
                }
                preEndIndex = content.IndexOf(">", preStartIndex);
                preCloseIndex = content.IndexOf("</pre>", preEndIndex);
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

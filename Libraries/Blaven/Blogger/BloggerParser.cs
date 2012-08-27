using System;
using System.Linq;
using System.Web;
using System.Xml.Linq;

using HtmlAgilityPack;

namespace Blaven.Blogger {
    internal static class BloggerParser {
        public static BlogData ParseBlogData(string blogKey, string bloggerDocumentContent) {
            try {
                return ParseBlogDataImpl(blogKey, bloggerDocumentContent);
            }
            catch(Exception ex) {
                throw new BloggerParsingException(blogKey, bloggerDocumentContent, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(string blogKey, string bloggerDocumentContent) {
            var document = new HtmlDocument();
            document.LoadHtml(bloggerDocumentContent);

            var root = document.DocumentNode.Element("feed");

            var blogEntries = root.SelectNodes("entry").Where(entry => !string.IsNullOrWhiteSpace(entry.InnerHtml));

            var posts = from entry in blogEntries
                        where entry.Element("title") != null
                        && entry.Elements("link").Any(el => el.Attributes["rel"].Value == "alternate")
                        select ParseEntry(blogKey, entry);

            var subtitle = root.Element("subtitle");
            var altLink = root.Elements("link").FirstOrDefault(el => el.Attributes["rel"].Value == "alternate");

            var blogInfo = new BlogInfo {
                BlogKey = blogKey,
                Subtitle = (subtitle != null) ? subtitle.InnerText : string.Empty,
                Title = root.Element("title").InnerText,
                Updated = ParseDate(root.Element("updated").InnerText),
                Url = (altLink != null) ? ((altLink.Attributes["href"] != null) ? altLink.Attributes["href"].Value : string.Empty) : string.Empty,
            };

            return new BlogData { Info = blogInfo, Posts = posts, };
        }

        private static BlogPost ParseEntry(string blogKey, HtmlNode entry) {
            var alternateLink = entry.Elements("link").FirstOrDefault(el => el.Attributes["rel"].Value == "alternate");

            string permaLinkFull = alternateLink == null ? string.Empty : alternateLink.Attributes["href"].Value;

            long id = ParseId(entry.Element("id").InnerText);
            string content = ParseContent(entry);

            var post = new BlogPost(blogKey, id) {
                Tags = entry.Elements("category").Select(cat => cat.Attributes["term"].Value),
                Content = content,
                PermaLinkAbsolute = permaLinkFull,
                PermaLinkRelative = GetRelativeUrl(permaLinkFull),
                Published = ParseDate(entry.Element("published").InnerText),
                Title = entry.Element("title").InnerText,
                Updated = ParseDate(entry.Element("updated").InnerText),
            };

            var authorNode = entry.Element("author");
            if(authorNode != null) {
                post.Author.Name = authorNode.Element("name").InnerText;

                var gdNs = XNamespace.Get("http://schemas.google.com/g/2005");
                post.Author.ImageUrl = authorNode.Element("gd:image").Attributes["src"].Value;
            }

            return post;
        }

        private static long ParseId(string val) {
            string findValue = ".post-";
            int index = val.IndexOf(findValue) + findValue.Length;

            string text = val.Substring(index);
            return long.Parse(text);
        }

        private static string ParseContent(HtmlNode entry) {
            string decodedContent = HttpUtility.HtmlDecode(entry.Element("content").InnerHtml);
            if(!decodedContent.Contains("</pre>")) {
                return decodedContent;
            }

            string parsedContent = decodedContent;

            int preStartIndex = decodedContent.IndexOf("<pre");
            int preEndIndex = (preStartIndex >= 0) ? decodedContent.IndexOf(">", preStartIndex) : -1;
            int preCloseIndex = (preStartIndex >= 0) ? decodedContent.IndexOf("</pre>", preEndIndex) : -1;

            while(preStartIndex >= 0 && preCloseIndex >= 0) {
                string preContent = parsedContent.Substring(preEndIndex + 1, preCloseIndex - preEndIndex);

                string encodedPre = HttpUtility.HtmlEncode(preContent);

                parsedContent = parsedContent.Remove(preEndIndex + 1, preCloseIndex - preEndIndex - 1);
                parsedContent = parsedContent.Insert(preEndIndex + 1, encodedPre);

                preStartIndex = decodedContent.IndexOf("<pre", preStartIndex + 1);
                if(preStartIndex < 0) {
                    break;
                }
                preEndIndex = decodedContent.IndexOf(">", preStartIndex);
                preCloseIndex = decodedContent.IndexOf("</pre>", preEndIndex);
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

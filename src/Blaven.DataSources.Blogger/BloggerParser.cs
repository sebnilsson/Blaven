using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Blaven.DataSources.Blogger
{
    internal static class BloggerParser
    {
        private const string BloggerIdPrefix = ".post-";

        public static BlogData ParseBlogData(BlavenBlogSetting settings, string xmlContent)
        {
            try
            {
                return ParseBlogDataImpl(settings, xmlContent);
            }
            catch (Exception ex)
            {
                throw new BloggerParserException(settings.BlogKey, xmlContent, ex);
            }
        }

        public static IEnumerable<BlogPost> ParseBlogPosts(BlavenBlogSetting settings, string xmlContent)
        {
            try
            {
                var bloggerDocument = XDocument.Parse(xmlContent);
                var posts = ParseBlogPostsImpl(settings, bloggerDocument);

                return posts.ToList();
            }
            catch (Exception ex)
            {
                throw new BloggerParserException(settings.BlogKey, xmlContent, ex);
            }
        }

        private static BlogData ParseBlogDataImpl(BlavenBlogSetting settings, string xmlContent)
        {
            var document = XDocument.Parse(xmlContent);
            var root = document.Root;
            if (root == null)
            {
                return null;
            }

            var ns = root.Name.Namespace;

            var subtitle = root.Element(ns + "subtitle");
            var altLink = root.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");

            string updatedText = root.TryGetElementValue(ns + "updated");
            var blogInfo = new BlogInfo
                               {
                                   BlogKey = settings.BlogKey,
                                   Subtitle = (subtitle != null) ? subtitle.Value : string.Empty,
                                   Title = root.TryGetElementValue(ns + "title"),
                                   Updated = ParseDate(updatedText),
                                   Url =
                                       (altLink != null)
                                           ? ((altLink.Attribute("href") != null)
                                                  ? altLink.Attribute("href").Value
                                                  : string.Empty)
                                           : string.Empty,
                               };

            var posts = ParseBlogPostsImpl(settings, document);

            return new BlogData { Info = blogInfo, Posts = posts.ToList() };
        }

        private static IEnumerable<BlogPost> ParseBlogPostsImpl(BlavenBlogSetting settings, XDocument document)
        {
            var root = document.Root;
            if (root == null)
            {
                return Enumerable.Empty<BlogPost>();
            }

            var ns = root.Name.Namespace;
            var entryElements = root.Elements(ns + "entry").Where(entry => !string.IsNullOrWhiteSpace(entry.Value));

            var posts = from entry in entryElements
                        let isValidEntry = GetIsValidEntry(entry, ns)
                        where isValidEntry
                        select ParseEntry(settings, ns, entry);
            return posts;
        }

        private static bool GetIsValidEntry(XElement entry, XNamespace ns)
        {
            //bool hasTitle = entry.GetElementExists(ns + "title");
            bool hasAltRel = entry.Elements(ns + "link").Any(el => el.TryGetAttributeValue("rel") == "alternate");
            return hasAltRel; //hasTitle && hasAltRel;
        }

        private static BlogPost ParseEntry(BlavenBlogSetting settings, XNamespace ns, XElement entry)
        {
            var gdNs = XNamespace.Get("http://schemas.google.com/g/2005");

            var alternateLink =
                entry.Elements(ns + "link").FirstOrDefault(el => el.Attribute("rel").Value == "alternate");
            string originalBloggerUrl = alternateLink == null ? string.Empty : alternateLink.Attribute("href").Value;

            string bloggerId = entry.TryGetElementValue(ns + "id");
            ulong parsedBloggerId = ParseBloggerId(bloggerId);

            string title = entry.TryGetElementValue(ns + "title");
            string updatedText = entry.TryGetElementValue(ns + "updated");

            string publishedText = entry.TryGetElementValue(ns + "published");
            var published = ParseDate(publishedText);

            var post = new BlogPost(settings.BlogKey, parsedBloggerId)
                           {
                               Tags =
                                   entry.Elements(ns + "category")
                                        .Select(
                                            cat => cat.Attribute("term").Value),
                               Content =
                                   entry.TryGetElementValue(ns + "content"),
                               DataSourceUrl = originalBloggerUrl,
                               Checksum =
                                   entry.TryGetAttributeValue(gdNs + "etag"),
                               Published = published,
                               Title = title,
                               Updated = ParseDate(updatedText),
                               UrlSlug = UrlSlug.Create(title),
                           };

            var authorNode = entry.Element(ns + "author");
            if (authorNode != null)
            {
                post.Author.Name = authorNode.TryGetElementValue(ns + "name");

                var imageNode = authorNode.Element(gdNs + "image");
                post.Author.ImageUrl = imageNode != null ? imageNode.Attribute("src").Value : null;
            }

            return post;
        }

        internal static ulong ParseBloggerId(string val)
        {
            int index = val.IndexOf(BloggerIdPrefix, StringComparison.InvariantCultureIgnoreCase)
                        + BloggerIdPrefix.Length;

            string text = val.Substring(index);
            return ulong.Parse(text);
        }

        private static DateTime ParseDate(string val)
        {
            var dateTime = DateTime.Parse(val).ToUniversalTime();
            return dateTime;
        }
    }
}
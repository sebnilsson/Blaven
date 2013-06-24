using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Google.GData.Blogger;
using Google.GData.Client;

namespace Blaven.DataSources.Blogger
{
    internal static class BloggerApiHelper
    {
        public const string BloggerFeedUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        public static string GetModifiedPostsContent(DataSourceRefreshContext refreshInfo)
        {
            var query = GetQuery(refreshInfo.BlogSetting);
            query.ModifiedSince = refreshInfo.LastRefresh ?? DateTime.MinValue;

            return GetBloggerDocument(refreshInfo.BlogSetting, query);
        }

        public static string GetBloggerDocument(BlavenBlogSetting setting, BloggerQuery query)
        {
            var service = GetService(setting.Username, setting.Password);

            string feedContent = GetFeedContent(setting, service, query);
            return feedContent;
        }

        public static IEnumerable<ulong> GetAllBloggerIds(BlavenBlogSetting setting)
        {
            var service = GetService(setting.Username, setting.Password);

            var query = GetQuery(setting);

            var feed = GetFeed(setting, service, query);

            return feed.Entries.Select(x => BloggerParser.ParseBloggerId(x.Id.Uri.Content));
        }

        private static BloggerService GetService(string username, string password)
        {
            var service = new BloggerService("Blaven");
            if (!string.IsNullOrWhiteSpace(username))
            {
                service.Credentials = new GDataCredentials(username, password);

                SetAuthForGoogleAppsUsers(service);
            }

            return service;
        }

        private static BloggerQuery GetQuery(BlavenBlogSetting setting)
        {
            string uri = (!string.IsNullOrWhiteSpace(setting.DataSourceUri))
                             ? setting.DataSourceUri
                             : string.Format(BloggerFeedUriFormat, setting.DataSourceId);

            var query = new BloggerQuery(uri) { NumberToRetrieve = int.MaxValue, OrderBy = "updated" };
            return query;
        }

        private static BloggerFeed GetFeed(BlavenBlogSetting setting, BloggerService service, BloggerQuery query)
        {
            if (query.Uri.IsFile)
            {
                service.RequestFactory = new LocalGDataRequestFactory();
            }

            try
            {
                return service.Query(query);
            }
            catch (Exception ex)
            {
                throw new BloggerApiHelperException(setting, ex);
            }
        }

        private static string GetFeedContent(BlavenBlogSetting setting, BloggerService service, BloggerQuery query)
        {
            var feed = GetFeed(setting, service, query);
            return GetFeedContent(feed);
        }

        private static string GetFeedContent(BloggerFeed feed)
        {
            using (var stream = new MemoryStream())
            {
                feed.SaveToXml(stream);
                stream.Position = 0;

                var reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                return content;
            }
        }

        private static void SetAuthForGoogleAppsUsers(BloggerService service)
        {
            var factory = service.RequestFactory as GDataGAuthRequestFactory;
            if (factory == null)
            {
                return;
            }
            
            // For proper authentication for Google Apps users
            factory.AccountType = "GOOGLE";
        }
    }
}
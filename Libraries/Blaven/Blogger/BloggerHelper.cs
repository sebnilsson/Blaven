﻿using System;
using System.Linq;
using System.Xml.Linq;

using Google.GData.Blogger;
using Google.GData.Client;

namespace Blaven.Blogger {
    internal static class BloggerHelper {
        public const string BloggerFeedUriFormat = "https://www.blogger.com/feeds/{0}/posts/default";

        public static XDocument GetBloggerDocument(BloggerSetting setting, DateTime? ifModifiedSince = null) {
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

        public static XDocument GetBloggerDocument(BloggerSetting setting, BloggerQuery query) {
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

        private static BloggerService GetBloggerService(string username, string password, bool isFile = false) {
            var service = new BloggerService("Blaven");
            if(!string.IsNullOrWhiteSpace(username)) {
                service.Credentials = new GDataCredentials(username, password);

                // For proper authentication for Google Apps users
                SetAuthForGoogleAppsUsers(service);
            }
            
            if(isFile) {
                service.RequestFactory = new Blaven.Blogger.LocalGDataRequestFactory();   
            }

            return service;
        }

        private static void SetAuthForGoogleAppsUsers(BloggerService service) {
            GDataGAuthRequestFactory factory = service.RequestFactory as GDataGAuthRequestFactory;
            if(factory == null) {
                return;
            }

            factory.AccountType = "GOOGLE";
        }
    }
}

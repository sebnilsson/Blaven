using System.Collections.Generic;
using System.Linq;

using Blaven.Blogger;
using Raven.Client;

namespace Blaven.Test {
    public static class BlogServiceTestHelper {
        public static BlogService GetBlogService(IDocumentStore documentStore, IEnumerable<string> blogKeys, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            var settings = from blogKey in blogKeys
                           let uri = XmlFilesTestHelper.GetProjectPath(blogKey + ".xml")
                           select new BloggerSetting {
                               BlogKey = blogKey,
                               BloggerUri = uri,
                           };

            var config = GetConfig(settings, documentStore, refreshAsync, ensureBlogsRefreshed);

            return new BlogService(config);
        }

        //public static BlogService GetBlogService(IDocumentStore documentStore, string blogKey, string fileName = null, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
        //    fileName = fileName ?? blogKey;

        //    string bloggerUri = XmlFilesTestHelper.GetProjectPath(fileName + ".xml");

        //    var settings = new[] {
        //        new BloggerSetting() {
        //            BlogKey = blogKey,
        //            BloggerUri = bloggerUri,
        //        }
        //    };

        //    var config = GetConfig(settings, documentStore, refreshAsync, ensureBlogsRefreshed);

        //    return new BlogService(config);
        //}

        public static BlogServiceConfig GetConfig(IEnumerable<string> blogKeys, IDocumentStore documentStore, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            var settings = from blogKey in blogKeys
                           let uri = XmlFilesTestHelper.GetProjectPath(blogKey + ".xml")
                           select new BloggerSetting {
                               BlogKey = blogKey,
                               BloggerUri = uri,
                           };

            return GetConfig(settings, documentStore, refreshAsync, ensureBlogsRefreshed);
        }

        public static BlogServiceConfig GetConfig(IEnumerable<BloggerSetting> settings, IDocumentStore documentStore, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            var config = new BlogServiceConfig(settings) {
                EnsureBlogsRefreshed = ensureBlogsRefreshed,
                DocumentStore = documentStore,
                RefreshAsync = refreshAsync,
            };

            return config;
        }
    }
}

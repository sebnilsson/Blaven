using System.Collections.Generic;
using System.Linq;

using Raven.Client;

namespace Blaven.Test
{
    public static class BlogServiceTestHelper
    {
        public static BlogService GetBlogService(
            IDocumentStore documentStore,
            IEnumerable<string> blogKeys,
            bool refreshAsync = true,
            bool ensureBlogsRefreshed = true)
        {
            var settings = GetBloggerSettings(blogKeys);

            var config = GetConfig(refreshAsync, ensureBlogsRefreshed);

            return new BlogService(documentStore, config, settings);
        }

        public static IEnumerable<BlavenBlogSetting> GetBloggerSettings(IEnumerable<string> blogKeys)
        {
            return from blogKey in blogKeys
                   let uri = XmlFilesTestHelper.GetProjectPath(blogKey + ".xml")
                   select new BlavenBlogSetting { BlogKey = blogKey, DataSourceUri = uri, };
        }

        public static BlogServiceConfig GetConfig(bool refreshAsync = true, bool ensureBlogsRefreshed = true)
        {
            var config = new BlogServiceConfig
                             {
                                 EnsureBlogsRefreshed = ensureBlogsRefreshed,
                                 RefreshAsync = refreshAsync,
                             };
            return config;
        }
    }
}
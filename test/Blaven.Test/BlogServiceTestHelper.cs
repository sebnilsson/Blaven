using System.Collections.Generic;
using System.Linq;

using Raven.Client;

namespace Blaven.Test
{
    using Blaven.DataSources.Blogger;

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
                   let uri = TestEnvironmentHelper.GetXmlFilePath(blogKey + ".xml")
                   select new BlavenBlogSetting<BloggerDataSource>(blogKey) { BlogKey = blogKey, DataSourceUri = uri, };
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
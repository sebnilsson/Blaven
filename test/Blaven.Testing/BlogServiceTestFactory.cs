using System.Collections.Generic;
using System.Linq;

using Blaven.Data.Tests;

namespace Blaven.Tests
{
    public static class BlogServiceTestFactory
    {
        public static BlogService Create(
            IEnumerable<BlogPost> blogPosts = null,
            IEnumerable<BlogMeta> blogMetas = null,
            params BlogSetting[] blogSettings)
        {
            var repository = new FakeRepository(blogPosts, blogMetas);

            blogSettings = (blogSettings != null && blogSettings.Any())
                               ? blogSettings
                               : BlogSettingTestData.CreateCollection().ToArray();

            var service = new BlogService(repository, blogSettings);
            return service;
        }
    }
}
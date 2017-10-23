using System.Collections.Generic;
using System.Linq;
using Blaven.BlogSources;
using Blaven.BlogSources.Testing;
using Blaven.DataStorage;
using Blaven.DataStorage.Testing;
using Blaven.Testing;

namespace Blaven.Synchronization.Testing
{
    public static class BlogSyncServiceTestFactory
    {
        public static BlogSyncService Create(
            IBlogSource blogSource = null,
            IDataStorage dataStorage = null,
            IEnumerable<BlogSetting> blogSettings = null)
        {
            blogSource = blogSource ?? new FakeBlogSource();
            dataStorage = dataStorage ?? new FakeDataStorage();
            blogSettings = blogSettings ?? BlogSettingTestData.CreateCollection();

            var service = new BlogSyncService(blogSource, dataStorage, blogSettings.ToArray());
            return service;
        }

        public static BlogSyncService CreateWithData(
            IEnumerable<BlogPost> blogSourcePosts = null,
            IEnumerable<BlogMeta> blogSourceMetas = null,
            IEnumerable<BlogPost> dataStoragePosts = null,
            IEnumerable<BlogSetting> blogSettings = null)
        {
            var blogSource = new FakeBlogSource(blogSourcePosts, blogSourceMetas);
            var dataStorage = new FakeDataStorage(dataStoragePosts);

            var service = Create(blogSource, dataStorage, blogSettings);
            return service;
        }
    }
}
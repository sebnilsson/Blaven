using System.Collections.Generic;
using System.Linq;

using Blaven.BlogSources;
using Blaven.Data;
using Blaven.Synchronization;

namespace Blaven.Tests
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
            blogSettings = blogSettings ?? Enumerable.Empty<BlogSetting>();

            var service = new BlogSyncService(blogSource, dataStorage, blogSettings.ToArray());
            return service;
        }
    }
}
using System;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public static BlogMeta GetBlogMeta(string blogKey = null)
        {
            var meta = new BlogMeta
                           {
                               BlogKey = blogKey ?? BlogKey,
                               Description = "Test Description",
                               Name = "Test_Name",
                               PublishedAt = new DateTime(2015, 2, 1),
                               SourceId = "SourceId-TEST",
                               UpdatedAt = new DateTime(2015, 3, 1),
                               Url = "http://testurl.com/testar/test.html"
                           };
            return meta;
        }
    }
}
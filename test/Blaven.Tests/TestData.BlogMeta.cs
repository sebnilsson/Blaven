using System;

namespace Blaven.Tests
{
    public static partial class TestData
    {
        public const string BlogMetaDescription = "Test Description";

        public const string BlogMetaName = "Test_Name";

        public const string BlogMetaSourceId = "SourceId-TEST";

        public static readonly DateTime BlogMetaPublishedAt = new DateTime(2015, 2, 1);

        public static readonly DateTime BlogMetaUpdatedAt = new DateTime(2015, 3, 1);

        public static BlogMeta GetBlogMeta(string blogKey = null)
        {
            var meta = new BlogMeta
                           {
                               BlogKey = blogKey ?? BlogKey,
                               Description = BlogMetaDescription,
                               Name = BlogMetaName,
                               PublishedAt = BlogMetaPublishedAt,
                               SourceId = BlogMetaSourceId,
                               UpdatedAt = BlogMetaUpdatedAt,
                               Url = "http://testurl.com/testar/test.html"
                           };
            return meta;
        }
    }
}
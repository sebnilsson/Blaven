using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Testing
{
    public static class BlogMetaTestData
    {
        public const string BlogKey = "test_blog-key"; //"Test_Blog-Key";
        public const string BlogKey1 = "test blog-key 1"; //"Test Blog-Key 1";
        public const string BlogKey2 = "test-blog key two"; //"Test-Blog Key TWO";
        public const string BlogKey3 = "blog-test key trees"; //"Blog-Test Key Trees";
        public const string BlogMetaDescription = "Test Description";
        public const string BlogMetaName = "Test_Name";
        public const string BlogMetaSourceId = "SourceId-TEST";

        public static string[] BlogKeys => new[] { BlogKey1, BlogKey2, BlogKey3 }.ToArray();

        public static DateTime BlogMetaPublishedAt => new DateTime(2015, 2, 1);

        public static DateTime BlogMetaUpdatedAt => new DateTime(2015, 3, 1);

        public static BlogMeta Create(string blogKey = null)
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

        public static IReadOnlyList<BlogMeta> CreateCollection(params string[] blogKeys)
        {
            var keys = blogKeys != null && blogKeys.Any() ? blogKeys : new[] { BlogKey }.Concat(BlogKeys);

            var metas = keys.Select(Create).ToReadOnlyList();
            return metas;
        }
    }
}
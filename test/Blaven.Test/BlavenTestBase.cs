using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Blaven.DataSources.Blogger;
using Blaven.RavenDb;
using Raven.Client;
using Raven.Client.Embedded;

namespace Blaven.Test
{
    public class BlavenTestBase
    {
        public const string TestBlogKey = "TEST";

        public const string TestBlogTitle = "TEST_BLOG_TITLE";

        private static string projectDirectory;

        public static readonly IEnumerable<string> TestBlogKeys = new[] { "TEST1", "TEST2" };

        public static string ProjectDirectory
        {
            get
            {
                if (projectDirectory == null)
                {
                    string codeBasePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(BlavenTestBase)).CodeBase);
                    string localPath = new Uri(codeBasePath).LocalPath;

                    projectDirectory = localPath;
                }
                return projectDirectory;
            }
        }

        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(
            string path = null, bool initWithIndexes = true)
        {
            path = !string.IsNullOrWhiteSpace(path) ? path : Guid.NewGuid().ToString();

            var documentStore = new EmbeddableDocumentStore
                                    {
                                        Configuration =
                                            {
                                                DataDirectory = path,
                                                RunInUnreliableYetFastModeThatIsNotSuitableForProduction
                                                    = true,
                                                DefaultStorageTypeName = "munin",
                                                RunInMemory = true,
                                            },
                                        RunInMemory = true,
                                    };

            if (initWithIndexes)
            {
                RavenDbHelper.InitWithIndexes(documentStore);
            }

            return documentStore;
        }

        public static BlogData GenerateBlogData(int postsCount, string blogKey = TestBlogKey)
        {
            var posts = GenerateBlogPosts(postsCount, blogKey);
            return GenerateBlogData(posts, blogKey);
        }

        public static BlogData GenerateBlogData(IEnumerable<BlogPost> posts = null, string blogKey = TestBlogKey)
        {
            posts = posts ?? Enumerable.Empty<BlogPost>();

            return new BlogData { Info = new BlogInfo { BlogKey = blogKey, Title = TestBlogTitle, }, Posts = posts, };
        }

        public static IEnumerable<BlogPost> GenerateBlogPosts(int count, string blogKey = TestBlogKey)
        {
            return GenerateBlogPosts(1, count, blogKey);
        }

        public static IEnumerable<BlogPost> GenerateBlogPosts(int start, int count, string blogKey = TestBlogKey)
        {
            var random = new Random();

            return from number in Enumerable.Range(start, count)
                   let published = DateTime.Now.AddMinutes(random.NextDouble() * 100)
                   select new BlogPost(blogKey, (uint)number) { Published = published };
        }

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
                   let uri = GetXmlFilePath(blogKey + ".xml")
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

        public static string GetDiskFilePath(params string[] relativeFilePaths)
        {
            relativeFilePaths = relativeFilePaths ?? Enumerable.Empty<string>().ToArray();

            string[] paths = new[] { "Resources", "DiskFiles", ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }

        public static string GetXmlFilePath(params string[] relativeFilePaths)
        {
            string[] paths = new[] { "XmlFiles", ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }
    }
}
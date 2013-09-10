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

        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(string path = null)
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
            documentStore.Initialize();

            RavenDbHelper.InitWithIndexes(documentStore);

            return documentStore;
        }

        public static BlogData GenerateBlogData(string blogKey, int postsCount)
        {
            var posts = GenerateBlogPosts(blogKey, postsCount);
            return GenerateBlogData(blogKey, posts);
        }

        public static BlogData GenerateBlogData(string blogKey, IEnumerable<BlogPost> posts = null)
        {
            posts = posts ?? Enumerable.Empty<BlogPost>();

            return new BlogData { Info = new BlogInfo { BlogKey = blogKey, Title = "TEST_TITLE", }, Posts = posts, };
        }

        public static IEnumerable<BlogPost> GenerateBlogPosts(string blogKey, int count)
        {
            return GenerateBlogPosts(blogKey, 1, count);
        }

        public static IEnumerable<BlogPost> GenerateBlogPosts(string blogKey, int start, int count)
        {
            var numbers = Enumerable.Range(start, count);
            return numbers.Select(number => new BlogPost(blogKey, (uint)number));
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
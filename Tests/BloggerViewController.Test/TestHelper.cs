using System;
using System.IO;
using System.Linq;
using System.Reflection;

using BloggerViewController.Configuration;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace BloggerViewController.Test {
    public static class TestHelper {
        private static string _projectDirectory;
        public static string ProjectDirectory {
            get {
                if(_projectDirectory == null) {
                    string codeBasePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TestHelper)).CodeBase);
                    string localPath = new Uri(codeBasePath).LocalPath;
                    var directoryInfo = new DirectoryInfo(localPath); // Remove /Debug
                    var projectDirectory = directoryInfo.Parent.Parent.FullName; // Remove /bin/Debug

                    _projectDirectory = localPath; //projectDirectory;
                }
                return _projectDirectory;
            }
        }

        public static string GetProjectPath(params string[] relativeFilePaths) {
            string[] paths = new[] { ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }

        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(string path) {
            var documentStore = new EmbeddableDocumentStore {
                Configuration = {
                    DataDirectory = path,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    DefaultStorageTypeName = "munin",
                    RunInMemory = true,
                },
                RunInMemory = true,
            };
            documentStore.Initialize();

            IndexCreation.CreateIndexes(
                typeof(BloggerViewController.Data.Indexes.BlogPostsOrderedByCreated).Assembly, documentStore);            

            return documentStore;
        }

        public static BlogService GetBlogService(IDocumentStore documentStore, string blogKey, string fileName = null) {
            fileName = fileName ?? blogKey;

            string bloggerUri = GetProjectPath(fileName + ".xml");

            var settings = new[] {
                new BloggerSetting() {
                    BlogKey = blogKey,
                    BloggerUri = bloggerUri,
                }
            };

            var config = new BlogServiceConfig(settings) {
                CacheTime = 0,
                DocumentStore = documentStore,
            };
            return new BlogService(config);
        }

        public static void WaitForIndexes(IDocumentStore documentStore) {
            while(documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0) {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}

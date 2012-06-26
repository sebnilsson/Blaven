using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Blaven.Blogger;
using Raven.Client;

namespace Blaven.Test {
    public static class BlogServiceTestHelper {
        private static string _projectDirectory;
        public static string ProjectDirectory {
            get {
                if(_projectDirectory == null) {
                    string codeBasePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(DocumentStoreTestHelper)).CodeBase);
                    string localPath = new Uri(codeBasePath).LocalPath;
                    var directoryInfo = new DirectoryInfo(localPath); // Remove /Debug
                    var projectDirectory = directoryInfo.Parent.Parent.FullName; // Remove /bin/Debug

                    _projectDirectory = localPath; //projectDirectory;
                }
                return _projectDirectory;
            }
        }

        public static string GetProjectPath(params string[] relativeFilePaths) {
            string[] paths = new[] { "XmlFiles", ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }

        public static BlogService GetBlogService(IDocumentStore documentStore, string blogKey, string fileName = null) {
            fileName = fileName ?? blogKey;

            string bloggerUri = BlogServiceTestHelper.GetProjectPath(fileName + ".xml");

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
    }
}

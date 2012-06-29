using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Blaven.Blogger;
using Raven.Client;
using System.Collections.Generic;

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

        public static BlogService GetBlogService(IDocumentStore documentStore, IEnumerable<string> blogKeys, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            var settings = from blogKey in blogKeys
                           let uri = BlogServiceTestHelper.GetProjectPath(blogKey + ".xml")
                           select new BloggerSetting {
                               BlogKey = blogKey,
                               BloggerUri = uri,
                           };

            var config = GetConfig(settings, documentStore, refreshAsync, ensureBlogsRefreshed);

            return new BlogService(config);
        }

        public static BlogService GetBlogService(IDocumentStore documentStore, string blogKey, string fileName = null, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            fileName = fileName ?? blogKey;
            
            string bloggerUri = BlogServiceTestHelper.GetProjectPath(fileName + ".xml");

            var settings = new[] {
                new BloggerSetting() {
                    BlogKey = blogKey,
                    BloggerUri = bloggerUri,
                }
            };

            var config = GetConfig(settings, documentStore, refreshAsync, ensureBlogsRefreshed);

            return new BlogService(config);
        }

        public static BlogServiceConfig GetConfig(IEnumerable<BloggerSetting> settings, IDocumentStore documentStore, bool refreshAsync = true, bool ensureBlogsRefreshed = true) {
            var config = new BlogServiceConfig(settings) {
                EnsureBlogsRefreshed = ensureBlogsRefreshed,
                DocumentStore = documentStore,
                RefreshAsync = refreshAsync,
            };

            return config;
        }
    }
}

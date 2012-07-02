using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blaven.Test {
    public static class XmlFilesTestHelper {
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
    }
}

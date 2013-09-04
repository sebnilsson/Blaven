using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blaven.Test
{
    public static class TestEnvironmentHelper
    {
        private static string projectDirectory;

        public static string ProjectDirectory
        {
            get
            {
                if (projectDirectory == null)
                {
                    string codeBasePath =
                        Path.GetDirectoryName(Assembly.GetAssembly(typeof(DocumentStoreTestHelper)).CodeBase);
                    string localPath = new Uri(codeBasePath).LocalPath;

                    projectDirectory = localPath;
                }
                return projectDirectory;
            }
        }

        public static string GetDiskFilePath(params string[] relativeFilePaths)
        {
            relativeFilePaths = relativeFilePaths ?? Enumerable.Empty<string>().ToArray();

            string[] paths = new[] { "DiskFiles", ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }

        public static string GetXmlFilePath(params string[] relativeFilePaths)
        {
            string[] paths = new[] { "XmlFiles", ProjectDirectory }.Concat(relativeFilePaths).ToArray();
            return Path.Combine(paths);
        }
    }
}
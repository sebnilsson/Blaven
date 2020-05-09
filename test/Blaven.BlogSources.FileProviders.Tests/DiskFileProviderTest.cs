using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Blaven.BlogSources.FileProviders.Tests
{
    public class DiskFileProviderTest
    {
        [Fact]
        public async Task GetFileData_FilesInDifferentFolderStructures_ReturnAllFiles()
        {
            // Arrange
            var fileDataProvider = GetDiskFileDataProvider("DiskResources");

            // Act
            var result = await fileDataProvider.GetFileData();

            // Assert
            var meta1 =
                result.Metas.FirstOrDefault(x => x.FolderName == "BlogKey1");
            var meta2 =
                result.Metas.FirstOrDefault(x => x.FolderName == "BlogKey2");
            var nonExistingMeta =
                result.Metas.FirstOrDefault(x =>
                    x.FolderName == "NON_EXISTING_BLOG_KEY");
            var post =
                result.Posts.FirstOrDefault(x => x.FolderName == "BlogKey1");
            var nonExistingPost =
                result.Posts.FirstOrDefault(x =>
                    x.FolderName == "NON_EXISTING_BLOG_KEY");

            Assert.True(meta1.Exists);
            Assert.True(meta2.Exists);
            Assert.True(post.Exists);
            Assert.False(nonExistingPost.Exists);
        }

        [Fact]
        public async Task GetFileData_NotRecursive_ReturnOnlyFolderFiles()
        {
            // Arrange
            var fileDataProvider =
                GetDiskFileDataProvider(
                    "DiskResources/SubFolder/BlogKey2",
                    recursive: false);

            // Act
            var result = await fileDataProvider.GetFileData();

            var meta1 =
                result.Metas.FirstOrDefault(x => x.FolderName == "BlogKey1");
            var meta2 =
                result.Metas.FirstOrDefault(x => x.FileName == ".meta.json");
            var post1 =
                result.Posts.FirstOrDefault(x => x.FolderName == "BlogKey1");
            var post2 =
                result.Posts.FirstOrDefault(x => x.FolderName == "BlogKey2");

            // Assert
            Assert.False(meta1.Exists);
            Assert.True(meta2.Exists);
            Assert.False(post1.Exists);
            Assert.False(post2.Exists);
        }

        private IFileDataProvider GetDiskFileDataProvider(
            string directoryPath,
            bool recursive = true)
        {
            var path =
                Path.Combine(Environment.CurrentDirectory, directoryPath);

            return
                new DiskFileDataProvider(
                    path,
                    recursive,
                    metaExtensions: new[] { ".json" },
                    postExtensions: new[] { ".md" });
        }
    }
}

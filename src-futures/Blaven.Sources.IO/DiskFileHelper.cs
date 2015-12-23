using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

using Blaven.Sources;

namespace Blaven.Sources.IO
{
    internal static class DiskFileHelper
    {
        public static IEnumerable<FileListItem> GetFileList(string dataSourceUri, params string[] acceptableExtensions)
        {
            string path = GetFullPath(dataSourceUri);

            var allFiles =
                Directory.GetFiles(path)
                         .Select(x => new FileInfo(x))
                         .Where(x => acceptableExtensions.Contains(x.Extension))
                         .Select(GetFileListItem);

            return allFiles;
        }

        public static IEnumerable<FileListItem> GetModifiedFiles(
            DataSourceRefreshContext refreshInfo, IEnumerable<FileListItem> fileList)
        {
            var blogPostsMeta = refreshInfo.ExistingBlogPostsMetas.ToList();
            var modifiedFiles = from file in fileList
                                let existing = blogPostsMeta.FirstOrDefault(x => x.Id == file.FullPath)
                                where existing != null || existing.Checksum != file.Checksum
                                select file;

            return modifiedFiles;
        }

        private static string GetFullPath(string path)
        {
            return (HttpContext.Current != null) ? HttpContext.Current.Server.MapPath(path) : path;
        }

        private static FileListItem GetFileListItem(FileInfo fileInfo)
        {
            return new FileListItem { Checksum = GetFileHash(fileInfo.FullName), FullPath = fileInfo.FullName };
        }

        private static string GetFileHash(string path)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            using (var fileReader = File.OpenRead(path))
            {
                var hash = sha1.ComputeHash(fileReader);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
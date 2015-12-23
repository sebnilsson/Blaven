using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Blaven.Sources;

namespace Blaven.Sources.IO
{
    public class DiskDataSource : DataSourceBase
    {
        public const string HtmlExtension = ".html";

        public const string JsonExtension = ".json";

        public const string BlogInfoFileName = "___blog-info.json";

        public override DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        {
            var allFiles = this.GetFileList(refreshInfo).ToList();

            var modifiedBlogPostMetas = this.GetModifiedPostMetas(refreshInfo, allFiles).ToList();

            var modifiedFiles = from file in allFiles
                                let fileName = Path.GetFileNameWithoutExtension(file.FullPath)
                                from modified in modifiedBlogPostMetas
                                let modifiedName = Path.GetFileNameWithoutExtension(modified.DataSourceId)
                                where fileName == modifiedName
                                select file;
            
            var modifiedBlogPosts = this.GetBlogPosts(refreshInfo, modifiedFiles).ToList();

            var allBloggerIds = allFiles.Select(x => x.FullPath);
            var removedBlogPostIds = this.GetDeletedPostIds(
                refreshInfo.ExistingBlogPostsMetas.Select(x => x.Id), allBloggerIds);

            var blogInfo = this.GetBlogInfo(allFiles, refreshInfo.BlogInfoChecksum);
            blogInfo.Updated = blogInfo.Updated
                               ?? (modifiedBlogPosts.Any()
                                       ? modifiedBlogPosts.Max(x => x.Updated) ?? DateTime.Now.ToUniversalTime()
                                       : (DateTime?)null);

            return new DataSourceRefreshResult
                       {
                           BlogInfo = blogInfo,
                           ModifiedBlogPosts = modifiedBlogPosts,
                           RemovedBlogPostIds = removedBlogPostIds.ToList()
                       };
        }

        protected internal virtual IEnumerable<FileListItem> GetFileList(DataSourceRefreshContext refreshInfo)
        {
            return DiskFileHelper.GetFileList(refreshInfo.BlogSetting.DataSourceUri, HtmlExtension, JsonExtension);
        }

        protected internal virtual string GetFileContent(string fullPath)
        {
            return File.ReadAllText(fullPath);
        }

        private IEnumerable<BlogPostMeta> GetModifiedPostMetas(
            DataSourceRefreshContext refreshInfo, IEnumerable<FileListItem> allFiles)
        {
            var metas = from kvp in GetContentMetaPairs(allFiles)
                        let dataSourceId = Path.GetFileNameWithoutExtension(kvp.Key.FullPath)
                        select
                            new BlogPostMeta(dataSourceId)
                                {
                                    BlogKey = refreshInfo.BlogSetting.BlogKey,
                                    Checksum = GetChecksum(kvp.Key, kvp.Value)
                                };

            return this.GetModifiedPostMetas(refreshInfo, metas);
        }

        private IEnumerable<BlogPost> GetBlogPosts(
            DataSourceRefreshContext refreshInfo, IEnumerable<FileListItem> modifiedFiles)
        {
            return GetContentMetaPairs(modifiedFiles).Select(x => this.GetBlogPost(refreshInfo, x.Key, x.Value));
        }

        private static IEnumerable<KeyValuePair<FileListItem, FileListItem>> GetContentMetaPairs(
            IEnumerable<FileListItem> modifiedFiles)
        {
            var fileList = modifiedFiles.ToList();

            var allNames =
                fileList.Where(x => Path.GetFileName(x.FullPath) != BlogInfoFileName)
                        .Select(x => Path.GetFileNameWithoutExtension(x.FullPath))
                        .Distinct()
                        .ToList();

            return from name in allNames
                   let contentFile =
                       fileList.FirstOrDefault(x => MatchNameAndExtension(x.FullPath, name, HtmlExtension))
                   let metaFile = fileList.FirstOrDefault(x => MatchNameAndExtension(x.FullPath, name, JsonExtension))
                   where contentFile != null
                   select new KeyValuePair<FileListItem, FileListItem>(contentFile, metaFile);
        }

        private BlogPost GetBlogPost(
            DataSourceRefreshContext refreshInfo, FileListItem contentFile, FileListItem metaFile)
        {
            if (contentFile == null)
            {
                throw new ArgumentNullException("contentFile");
            }

            string blogKey = refreshInfo.BlogSetting.BlogKey;

            string content = this.GetFileContent(contentFile.FullPath);
            string metaContent = (metaFile != null) ? this.GetFileContent(metaFile.FullPath) : null;

            var blogPost = ((!String.IsNullOrWhiteSpace(metaContent))
                                ? JsonHelper.Deserialize<BlogPost>(metaContent)
                                : null) ?? new BlogPost(blogKey);

            blogPost.BlogKey = blogPost.BlogKey ?? refreshInfo.BlogSetting.BlogKey;
            blogPost.Content = content;
            blogPost.DataSourceUrl = contentFile.FullPath;
            blogPost.Title = blogPost.Title ?? Path.GetFileNameWithoutExtension(contentFile.FullPath);
            blogPost.UrlSlug = blogPost.UrlSlug ?? UrlSlug.Create(blogPost.Title);

            blogPost.Checksum = GetChecksum(contentFile, metaFile);

            string dataSourceId = Path.GetFileNameWithoutExtension(contentFile.FullPath);
            blogPost.SetIds(dataSourceId);

            return blogPost;
        }

        private static string GetChecksum(FileListItem contentFile, FileListItem metaFile)
        {
            string contentChecksum = contentFile.Checksum;
            string metaChecksum = (metaFile != null) ? metaFile.Checksum : null;
            return string.Format("{0}{1}", contentChecksum, metaChecksum);
        }

        private BlogInfo GetBlogInfo(IEnumerable<FileListItem> allFiles, string blogInfoChecksum)
        {
            var blogInfoFile = allFiles.FirstOrDefault(x => Path.GetFileName(x.FullPath) == BlogInfoFileName);
            if (blogInfoFile == null)
            {
                throw new ArgumentNullException("allFiles", "No file found for blog-info.");
            }

            if (blogInfoFile.Checksum == blogInfoChecksum)
            {
                return null;
            }

            var blogInfoContent = this.GetFileContent(blogInfoFile.FullPath);
            return JsonHelper.Deserialize<BlogInfo>(blogInfoContent);
        }

        private static bool MatchNameAndExtension(string path, string matchName, string matchExtension)
        {
            return Path.GetFileNameWithoutExtension(path) == matchName && Path.GetExtension(path) == matchExtension;
        }
    }
}
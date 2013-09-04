using System.Collections.Generic;
using System.Linq;

using Blaven.DataSources.Disk;

namespace Blaven.DataSources.GitHub
{
    public class GitHubDataSource : DiskDataSource
    {
        private GitHubApiHelper apiHelper;

        public override DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        {
            this.apiHelper = new GitHubApiHelper(refreshInfo.BlogSetting);

            return base.Refresh(refreshInfo);
        }

        protected override IEnumerable<FileListItem> GetFileList(DataSourceRefreshContext refreshInfo)
        {
            return
                this.apiHelper.GetFileList(HtmlExtension, JsonExtension)
                    .Select(x => new FileListItem { Checksum = x.Sha, FullPath = x.Path });
        }

        protected override string GetFileContent(string fullPath)
        {
            return this.apiHelper.GetFileContent(fullPath);
        }
    }
}
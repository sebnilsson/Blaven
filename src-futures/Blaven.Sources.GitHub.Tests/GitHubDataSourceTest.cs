using System.Configuration;
using System.Linq;

using Blaven.DataSources.Disk.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.DataSources.GitHub.Tests
{
    [TestClass]
    public class GitHubDataSourceTest : DiskDataSourceTest
    {
        internal override BlavenBlogSetting GetSettings(IDataSource dataSource)
        {
            string gitHubPassword = ConfigurationManager.AppSettings["Dropbox.Password"];
            string gitHubUsername = ConfigurationManager.AppSettings["Dropbox.Username"];

            return new BlavenBlogSetting(BlogKey, dataSource)
                       {
                           DataSourceId = "Sebnilsson.com",
                           DataSourceUri = "test/_BlogPosts",
                           Username = gitHubUsername,
                           Password = gitHubPassword
                       };
        }

        [TestMethod]
        public void Refresh_PostsWithMetaFiles_ShouldOverrideTitle()
        {
            var dataSource = new GitHubDataSource();

            var result = this.PostsWithMetaFiles(dataSource);

            var post1 = result.ModifiedBlogPosts.FirstOrDefault(x => x.DataSourceId == "blog-post-1");
            var post2 = result.ModifiedBlogPosts.FirstOrDefault(x => x.DataSourceId == "blog-post-2");

            Assert.IsNotNull(post1);
            Assert.IsNotNull(post2);

            Assert.AreEqual(post1.Title, "TESTPOST_1");
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithSameData_ShouldNotReturnAnyModifiedPosts()
        {
            var dataSource = new GitHubDataSource();

            var result = this.SecondRefreshWithSameData(dataSource);

            Assert.AreEqual(result.Item1.ModifiedBlogPosts.Count(), 2);
            Assert.AreEqual(result.Item2.ModifiedBlogPosts.Count(), 0);
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithModifiedChecksum_ShouldReturnModifiedPosts()
        {
            var dataSource = new GitHubDataSource();

            var result = this.SecondRefreshWithModifiedChecksum(dataSource);

            Assert.AreEqual(result.Item1.Count(), 2);
            Assert.AreEqual(result.Item2.Count(), 2);
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithSameData_ShouldKeepBlogInfo()
        {
            var dataSource = new GitHubDataSource();

            var result = this.SecondRefreshWithSameData(dataSource);

            Assert.AreEqual(result.Item1.BlogInfo.Title, "TESTBLOG_TITLE");
            Assert.AreEqual(result.Item2.BlogInfo.Title, "TESTBLOG_TITLE");
        }
    }
}
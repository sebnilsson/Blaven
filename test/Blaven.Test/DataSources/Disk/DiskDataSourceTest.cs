using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.Test;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blaven.DataSources.Disk.Test
{
    [TestClass]
    public class DiskDataSourceTest : BlavenTestBase
    {
        protected internal const string BlogKey = "TESTBLOGKEY";

        internal virtual BlavenBlogSetting GetSettings(IDataSource dataSource)
        {
            string dataSourceUri = GetDiskFilePath();
            return new BlavenBlogSetting(BlogKey, dataSource) { DataSourceUri = dataSourceUri };
        }

        [TestMethod]
        public void Refresh_PostsWithMetaFiles_ShouldOverrideTitle()
        {
            var dataSource = new DiskDataSource();

            var result = this.PostsWithMetaFiles(dataSource);

            var post1 = result.ModifiedBlogPosts.FirstOrDefault(x => x.DataSourceId == "blog-post-1");
            var post2 = result.ModifiedBlogPosts.FirstOrDefault(x => x.DataSourceId == "blog-post-2");

            Assert.IsNotNull(post1);
            Assert.IsNotNull(post2);

            Assert.AreEqual(post1.Title, "TESTPOST_1");
        }

        internal DataSourceRefreshResult PostsWithMetaFiles(IDataSource dataSource)
        {
            var context = new DataSourceRefreshContext
                              {
                                  ExistingBlogPostsMetas = Enumerable.Empty<BlogPostMeta>(),
                                  BlogSetting = this.GetSettings(dataSource)
                              };

            return dataSource.Refresh(context);
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithSameData_ShouldNotReturnAnyModifiedPosts()
        {
            var dataSource = new DiskDataSource();

            var result = this.SecondRefreshWithSameData(dataSource);

            Assert.AreEqual(result.Item1.ModifiedBlogPosts.Count(), 2);
            Assert.AreEqual(result.Item2.ModifiedBlogPosts.Count(), 0);
        }

        internal Tuple<DataSourceRefreshResult, DataSourceRefreshResult> SecondRefreshWithSameData(
            IDataSource dataSource)
        {
            var context = new DataSourceRefreshContext
                              {
                                  ExistingBlogPostsMetas = Enumerable.Empty<BlogPostMeta>(),
                                  BlogSetting = this.GetSettings(dataSource)
                              };

            var result1 = dataSource.Refresh(context);

            context.ExistingBlogPostsMetas = result1.ModifiedBlogPosts.Select(x => new BlogPostMeta(x)).ToList();

            var result2 = dataSource.Refresh(context);

            return new Tuple<DataSourceRefreshResult, DataSourceRefreshResult>(result1, result2);
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithModifiedChecksum_ShouldReturnModifiedPosts()
        {
            var dataSource = new DiskDataSource();

            var result = this.SecondRefreshWithModifiedChecksum(dataSource);

            Assert.AreEqual(result.Item1.Count(), 2);
            Assert.AreEqual(result.Item2.Count(), 2);
        }

        internal Tuple<IEnumerable<BlogPost>, IEnumerable<BlogPost>> SecondRefreshWithModifiedChecksum(
            IDataSource dataSource)
        {
            var context = new DataSourceRefreshContext
                              {
                                  ExistingBlogPostsMetas = Enumerable.Empty<BlogPostMeta>(),
                                  BlogSetting = this.GetSettings(dataSource)
                              };

            var result1 = dataSource.Refresh(context);
            var modified1 = result1.ModifiedBlogPosts;

            context.ExistingBlogPostsMetas = result1.ModifiedBlogPosts.Select(x => new BlogPostMeta(x)).ToList();
            context.ExistingBlogPostsMetas.ElementAt(0).Checksum = "TEST_CHECKSUM1";
            context.ExistingBlogPostsMetas.ElementAt(1).Checksum = "TEST_CHECKSUM2";

            var result2 = dataSource.Refresh(context);
            var modified2 = result2.ModifiedBlogPosts;

            return new Tuple<IEnumerable<BlogPost>, IEnumerable<BlogPost>>(modified1, modified2);
        }

        [TestMethod]
        public void Refresh_SecondRefreshWithSameData_ShouldKeepBlogInfo()
        {
            var dataSource = new DiskDataSource();

            var result = this.SecondRefreshWithSameData(dataSource);

            Assert.AreEqual(result.Item1.BlogInfo.Title, "TESTBLOG_TITLE");
            Assert.AreEqual(result.Item2.BlogInfo.Title, "TESTBLOG_TITLE");
        }
    }
}
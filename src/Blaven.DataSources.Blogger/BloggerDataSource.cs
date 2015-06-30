using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb;

namespace Blaven.DataSources.Blogger
{
    public class BloggerDataSource : IDataSource
    {
        public DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        {
            // TODO: Wait for Blogger API-bug to be fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555

            var bloggerBlog = BloggerApiHelper.GetBlogInfo(refreshInfo);
            var modifiedPosts = BloggerApiHelper.GetModifiedPostsContent(refreshInfo);

            var blogData = BloggerParser.ParseBlogData(refreshInfo.BlogSetting, bloggerBlog, modifiedPosts);

            var existingBlogPostsMeta = refreshInfo.ExistingBlogPostsMetas.ToList();
            var modifiedBlogPosts = from post in blogData.Posts
                                    let existing =
                                        existingBlogPostsMeta.FirstOrDefault(
                                            x => x.Id == post.Id && x.DataSourceId == post.DataSourceId)
                                    where existing == null || existing.Checksum != post.Checksum
                                    select post;

            var allBloggerIds = BloggerApiHelper.GetAllBloggerIds(refreshInfo.BlogSetting);
            var removedBlogPostIds = GetDeletedPostIds(existingBlogPostsMeta.Select(x => x.Id), allBloggerIds);

            return new DataSourceRefreshResult
                       {
                           BlogInfo = blogData.Info,
                           ModifiedBlogPosts = modifiedBlogPosts.ToList(),
                           RemovedBlogPostIds = removedBlogPostIds
                       };
        }

        private static IEnumerable<string> GetDeletedPostIds(
            IEnumerable<string> repositoryIds, IEnumerable<ulong> allBloggerIds)
        {
            var allBloggerIdRavenIds = from bloggerId in allBloggerIds
                                       let blavenHash = BlavenHelper.GetBlavenHash(bloggerId)
                                       let ravenId = RavenDbHelper.GetEntityId<BlogPost>(blavenHash)
                                       select ravenId;

            //var dataSourceIds =
            //    allBloggerIds.Select(x => RavenDbHelper.GetEntityId<BlogPost>(BlavenHelper.GetBlavenHash(x)));
            var deletedIds = repositoryIds.Where(x => !allBloggerIdRavenIds.Contains(x));
            return deletedIds.ToList();
        }
    }
}
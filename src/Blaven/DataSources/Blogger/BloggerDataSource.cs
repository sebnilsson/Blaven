using System.Collections.Generic;
using System.Linq;

using Blaven.RavenDb;

namespace Blaven.DataSources.Blogger
{
    public class BloggerDataSource : IBlogDataSource
    {
        public DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        {
            // TODO: Wait for Blogger API-bug to be fixed:
            // http://code.google.com/p/gdata-issues/issues/detail?id=2555

            string modifiedPostsContent = BloggerApiHelper.GetModifiedPostsContent(refreshInfo);
            var blogData = BloggerParser.ParseBlogData(refreshInfo.BlogSetting, modifiedPostsContent);

            var blogPostsMeta = refreshInfo.BlogPostsMetas.ToList();
            var modifiedBlogPosts = from post in blogData.Posts
                                    let existing =
                                        blogPostsMeta.FirstOrDefault(
                                            x => x.Id == post.Id && x.DataSourceId == post.DataSourceId)
                                    where existing == null || existing.Checksum != post.Checksum
                                    select post;

            var allBloggerIds = BloggerApiHelper.GetAllBloggerIds(refreshInfo.BlogSetting);
            var removedBlogPostIds = GetDeletedPostIds(blogPostsMeta.Select(x => x.Id), allBloggerIds);

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
            var dataSourceIds =
                allBloggerIds.Select(x => RavenDbHelper.GetEntityId<BlogPost>(BlavenHelper.GetBlavenHash(x)));
            var deletedIds = repositoryIds.Where(x => !dataSourceIds.Contains(x));
            return deletedIds.ToList();
        }
    }
}
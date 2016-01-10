using System;
using System.Collections.Generic;

using Google.Apis.Blogger.v3;
using Google.Apis.Services;

namespace Blaven.BlogSources.Blogger
{
    public class BloggerDataSource : IBlogSource
    {
        private readonly BloggerApiHelper bloggerApiHelper;

        public BloggerDataSource(string apiKey)
            : this(GetDefaultBloggerService(apiKey))
        {
        }

        internal BloggerDataSource(BloggerService bloggerService)
        {
            if (bloggerService == null)
            {
                throw new ArgumentNullException(nameof(bloggerService));
            }

            this.bloggerApiHelper = new BloggerApiHelper(bloggerService);
        }

        public BlogMeta GetMeta(BlogSetting blogSetting)
        {
            if (blogSetting == null)
            {
                throw new ArgumentNullException(nameof(blogSetting));
            }

            var meta = this.bloggerApiHelper.GetMeta(blogSetting);
            return meta;
        }

        public BlogSourceChangeSet GetChanges(BlogSetting blogSetting, IEnumerable<BlogPostBase> dbBlogPosts)
        {
            var changes = this.bloggerApiHelper.GetChanges(blogSetting, dbBlogPosts);
            return changes;
        }

        //public DataSourceRefreshResult Refresh(DataSourceRefreshContext refreshInfo)
        //{
        //    // TODO: Wait for Blogger API-bug to be fixed:
        //    // http://code.google.com/p/gdata-issues/issues/detail?id=2555

        //    var bloggerBlog = BloggerApiHelper.GetBlogInfo(refreshInfo);
        //    var modifiedPosts = BloggerApiHelper.GetModifiedPostsContent(refreshInfo);

        //    var blogData = BloggerParser.ParseBlogData(refreshInfo.BlogSetting, bloggerBlog, modifiedPosts);

        //    var existingBlogPostsMeta = refreshInfo.ExistingBlogPostsMetas.ToList();
        //    var modifiedBlogPosts = from post in blogData.Posts
        //                            let existing =
        //                                existingBlogPostsMeta.FirstOrDefault(
        //                                    x => x.Id == post.Id && x.DataSourceId == post.DataSourceId)
        //                            where existing == null || existing.Checksum != post.Checksum
        //                            select post;

        //    var allBloggerIds = BloggerApiHelper.GetAllBloggerIds(refreshInfo.BlogSetting);
        //    var removedBlogPostIds = GetDeletedPostIds(existingBlogPostsMeta.Select(x => x.Id), allBloggerIds);

        //    return new DataSourceRefreshResult
        //               {
        //                   BlogInfo = blogData.Info,
        //                   ModifiedBlogPosts = modifiedBlogPosts.ToList(),
        //                   RemovedBlogPostIds = removedBlogPostIds
        //               };
        //}

        //private static IEnumerable<string> GetDeletedPostIds(
        //    IEnumerable<string> repositoryIds, IEnumerable<ulong> allBloggerIds)
        //{
        //    var allBloggerIdRavenIds = from bloggerId in allBloggerIds
        //                               let blavenHash = BlavenHelper.GetBlavenHash(bloggerId)
        //                               let ravenId = RavenDbHelper.GetEntityId<BlogPost>(blavenHash)
        //                               select ravenId;

        //    var deletedIds = repositoryIds.Where(x => !allBloggerIdRavenIds.Contains(x));
        //    return deletedIds.ToList();
        //}

        private static BloggerService GetDefaultBloggerService(string apiKey)
        {
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var initializer = new BaseClientService.Initializer { ApiKey = apiKey, ApplicationName = "Blaven" };
            var service = new BloggerService(initializer);

            return service;
        }
    }
}
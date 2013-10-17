using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataSources;
using Raven.Client;

namespace Blaven.RavenDb
{
    internal class RepositoryRefreshService
    {
        private readonly Repository repository;

        private readonly string blogKey;

        private readonly bool throwOnCritical;

        public RepositoryRefreshService(Repository repository, string blogKey, bool throwOnCritical = false)
        {
            this.repository = repository;
            this.blogKey = blogKey;
            this.throwOnCritical = throwOnCritical;
        }

        internal async Task Refresh(BlogData blogData)
        {
            var refreshResult = new DataSourceRefreshResult
                                    {
                                        BlogInfo = blogData.Info,
                                        ModifiedBlogPosts = blogData.Posts
                                    };
            await Refresh(refreshResult);
        }

        public async Task Refresh(DataSourceRefreshResult refreshResult)
        {
            if (refreshResult == null)
            {
                throw new ArgumentNullException("refreshResult");
            }

            refreshResult.ModifiedBlogPosts = refreshResult.ModifiedBlogPosts ?? Enumerable.Empty<BlogPost>();
            refreshResult.RemovedBlogPostIds = refreshResult.RemovedBlogPostIds ?? Enumerable.Empty<string>();

            try
            {
                using (var refreshSession = repository.GetMaxRequestSessionAsync())
                {
                    refreshResult.ModifiedBlogPosts = this.RemoveDuplicatePosts(
                        refreshResult.ModifiedBlogPosts);

                    this.RefreshBlogInfo(refreshSession, refreshResult.BlogInfo);

                    this.RefreshBlogPosts(refreshSession, refreshResult.ModifiedBlogPosts);

                    FlagDeletedBlogPosts(refreshSession, refreshResult.RemovedBlogPostIds);

                    this.UpdateBlogRefresh(refreshSession);

                    await refreshSession.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryRefreshServiceException(blogKey, ex);
            }
        }

        private IEnumerable<BlogPost> RemoveDuplicatePosts(IEnumerable<BlogPost> modifiedPosts)
        {
            var modifiedPostList = modifiedPosts.ToList();

            var postMeta = repository.GetAllBlogPostMeta(this.blogKey);
            var duplicateItems = (from modified in modifiedPostList
                                  where
                                      postMeta.Any(x => x.Id == modified.Id && x.DataSourceId != modified.DataSourceId)
                                  select modified).ToList();

            if (!duplicateItems.Any())
            {
                return modifiedPostList;
            }

            if (this.throwOnCritical)
            {
                var duplicatePost = modifiedPostList.First();
                string exceptionMessage =
                    string.Format(
                        "Duplicate calculated ID by Blaven for post with title '{0}'."
                        + " Move content of post to a new item to get a new calculated ID.",
                        duplicatePost.Title);
                throw new BlavenBlogException(this.blogKey, message: exceptionMessage);
            }

            duplicateItems.ForEach(x => modifiedPostList.Remove(x));

            return modifiedPostList;
        }

        private async void RefreshBlogInfo(IAsyncDocumentSession session, BlogInfo updateInfo)
        {
            if (updateInfo == null)
            {
                return;
            }

            string blogInfoUrl = RavenDbHelper.GetEntityId<BlogInfo>(this.blogKey);
            var blogInfo = await session.LoadAsync<BlogInfo>(blogInfoUrl);

            if (blogInfo == null)
            {
                blogInfo = new BlogInfo { BlogKey = blogKey };
                await session.StoreAsync(blogInfo, blogInfoUrl);
            }

            blogInfo.Subtitle = updateInfo.Subtitle;
            blogInfo.Title = updateInfo.Title;
            blogInfo.Updated = updateInfo.Updated;
            blogInfo.Url = updateInfo.Url;
        }

        private async void RefreshBlogPosts(IAsyncDocumentSession session, IEnumerable<BlogPost> modifiedPosts)
        {
            var posts = modifiedPosts.ToList();
            if (!posts.Any())
            {
                return;
            }

            var updatedPostsList = posts.OrderBy(x => x.Id).ToList();
            var updatedPostsIds = updatedPostsList.Select(GetPostBlavenId).ToList();

            var storedPosts = await session.LoadAsync<BlogPost>(updatedPostsIds);
            for (int i = 0; i < storedPosts.Length; i++)
            {
                var storedPost = storedPosts[i];
                var updatedPost = updatedPostsList[i];

                if (storedPost == null)
                {
                    storedPost = updatedPost;
                    await session.StoreAsync(storedPost);
                }
                else
                {
                    storedPost.Author = updatedPost.Author;
                    storedPost.BlavenId = updatedPost.BlavenId;
                    storedPost.BlogKey = updatedPost.BlogKey;
                    storedPost.Checksum = updatedPost.Checksum;
                    storedPost.Content = updatedPost.Content;
                    storedPost.DataSourceId = updatedPost.DataSourceId;
                    storedPost.DataSourceUrl = updatedPost.DataSourceUrl;
                    storedPost.IsDeleted = updatedPost.IsDeleted;
                    storedPost.Published = updatedPost.Published;
                    storedPost.Tags = updatedPost.Tags;
                    storedPost.Title = updatedPost.Title;
                    storedPost.Updated = updatedPost.Updated;
                    storedPost.UrlSlug = updatedPost.UrlSlug;
                }
            }
        }

        private static string GetPostBlavenId(BlogPost blogPost)
        {
            return !string.IsNullOrWhiteSpace(blogPost.Id)
                       ? blogPost.Id
                       : BlavenHelper.GetBlavenHash(blogPost.DataSourceId);
        }

        private async void UpdateBlogRefresh(IAsyncDocumentSession session)
        {
            string blogRefreshId = RavenDbHelper.GetEntityId<BlogRefresh>(this.blogKey);
            var blogRefresh = await session.LoadAsync<BlogRefresh>(blogRefreshId);

            if (blogRefresh == null)
            {
                blogRefresh = new BlogRefresh { BlogKey = this.blogKey };
                await session.StoreAsync(blogRefresh, blogRefreshId);
            }
            blogRefresh.Timestamp = DateTime.Now;
        }

        private static async void FlagDeletedBlogPosts(IAsyncDocumentSession session, IEnumerable<string> blogPostIds)
        {
            var deletedPosts = await session.LoadAsync<BlogPost>(blogPostIds);
            foreach (var deletedPost in deletedPosts)
            {
                deletedPost.IsDeleted = true;
            }
        }
    }
}
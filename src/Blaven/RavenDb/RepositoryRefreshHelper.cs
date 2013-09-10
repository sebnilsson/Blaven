using System;
using System.Collections.Generic;
using System.Linq;

using Blaven.DataSources;
using Raven.Client;

namespace Blaven.RavenDb
{
    internal static class RepositoryRefreshHelper
    {
        internal static void Refresh(
            Repository repository, string blogKey, BlogData blogData, bool throwOnCritical = false)
        {
            var refreshResult = new DataSourceRefreshResult
                                    {
                                        BlogInfo = blogData.Info,
                                        ModifiedBlogPosts = blogData.Posts
                                    };
            Refresh(repository, blogKey, refreshResult, throwOnCritical);
        }

        public static void Refresh(
            Repository repository, string blogKey, DataSourceRefreshResult refreshResult, bool throwOnException = false)
        {
            if (refreshResult == null)
            {
                throw new ArgumentNullException("refreshResult");
            }

            refreshResult.ModifiedBlogPosts = refreshResult.ModifiedBlogPosts ?? Enumerable.Empty<BlogPost>();
            refreshResult.RemovedBlogPostIds = refreshResult.RemovedBlogPostIds ?? Enumerable.Empty<string>();

            try
            {
                using (var refreshSession = repository.GetMaxRequestSession())
                {
                    refreshResult.ModifiedBlogPosts = RemoveDuplicatePosts(
                        repository, blogKey, refreshResult.ModifiedBlogPosts, throwOnException);

                    RefreshBlogInfo(refreshSession, blogKey, refreshResult.BlogInfo);

                    RefreshBlogPosts(refreshSession, refreshResult.ModifiedBlogPosts);

                    FlagDeletedBlogPosts(refreshSession, refreshResult.RemovedBlogPostIds);

                    UpdateBlogRefresh(refreshSession, blogKey);

                    refreshSession.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryRefreshException(blogKey, ex);
            }
        }

        private static IEnumerable<BlogPost> RemoveDuplicatePosts(
            Repository repository, string blogKey, IEnumerable<BlogPost> modifiedPosts, bool throwOnException)
        {
            var modifiedPostList = modifiedPosts.ToList();

            var postMeta = repository.GetAllBlogPostMeta(blogKey);
            var duplicateItems = (from modified in modifiedPostList
                                  where
                                      postMeta.Any(x => x.Id == modified.Id && x.DataSourceId != modified.DataSourceId)
                                  select modified).ToList();

            if (!duplicateItems.Any())
            {
                return modifiedPostList;
            }

            if (throwOnException)
            {
                var duplicatePost = modifiedPostList.First();
                string exceptionMessage =
                    string.Format(
                        "Duplicate calculated ID by Blaven for post with title '{0}'."
                        + " Move content of post to a new item to get a new calculated ID.",
                        duplicatePost.Title);
                throw new BlavenBlogException(blogKey, message: exceptionMessage);
            }

            duplicateItems.ForEach(x => modifiedPostList.Remove(x));

            return modifiedPostList;
        }

        private static void RefreshBlogInfo(IDocumentSession session, string blogKey, BlogInfo updateInfo)
        {
            if (updateInfo == null)
            {
                return;
            }

            string blogInfoUrl = RavenDbHelper.GetEntityId<BlogInfo>(blogKey);
            var blogInfo = session.Load<BlogInfo>(blogInfoUrl);

            if (blogInfo == null)
            {
                blogInfo = new BlogInfo { BlogKey = blogKey };
                session.Store(blogInfo, blogInfoUrl);
            }

            blogInfo.Subtitle = updateInfo.Subtitle;
            blogInfo.Title = updateInfo.Title;
            blogInfo.Updated = updateInfo.Updated;
            blogInfo.Url = updateInfo.Url;
        }

        private static void RefreshBlogPosts(IDocumentSession session, IEnumerable<BlogPost> modifiedPosts)
        {
            var posts = modifiedPosts.ToList();
            if (!posts.Any())
            {
                return;
            }

            var updatedPostsList = posts.OrderBy(x => x.Id).ToList();
            var updatedPostsIds = updatedPostsList.Select(GetPostRavenId).ToList();

            var storedPosts = session.Load<BlogPost>(updatedPostsIds);
            for (int i = 0; i < storedPosts.Length; i++)
            {
                var storedPost = storedPosts[i];
                var updatedPost = updatedPostsList[i];

                if (storedPost == null)
                {
                    storedPost = updatedPost;
                    session.Store(storedPost);
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

        private static string GetPostRavenId(BlogPost blogPost)
        {
            return !string.IsNullOrWhiteSpace(blogPost.Id)
                       ? blogPost.Id
                       : BlavenHelper.GetBlavenHash(blogPost.DataSourceId);
        }

        private static void UpdateBlogRefresh(IDocumentSession session, string blogKey)
        {
            string blogRefreshId = RavenDbHelper.GetEntityId<BlogRefresh>(blogKey);
            var blogRefresh = session.Load<BlogRefresh>(blogRefreshId);

            if (blogRefresh == null)
            {
                blogRefresh = new BlogRefresh { BlogKey = blogKey };
                session.Store(blogRefresh, blogRefreshId);
            }
            blogRefresh.Timestamp = DateTime.Now;
        }

        private static void FlagDeletedBlogPosts(IDocumentSession session, IEnumerable<string> blogPostIds)
        {
            var deletedPosts = session.Load<BlogPost>(blogPostIds);
            foreach (var deletedPost in deletedPosts)
            {
                deletedPost.IsDeleted = true;
            }
        }
    }
}
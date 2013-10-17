using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataSources;
using Blaven.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace Blaven.RavenDb
{
    internal class Repository : IDisposable
    {
        private const int WaitForDataTimeoutSeconds = 5;

        private const int WaitForPostsTimeoutSeconds = 3;

        private readonly IDocumentStore documentStore;

        private readonly RequestLazy<IDocumentSession> currentSessionLazy;

        public Repository(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
            this.currentSessionLazy = new RequestLazy<IDocumentSession>(() => this.documentStore.OpenSession());
        }

        internal IDocumentSession CurrentSession
        {
            get
            {
                return this.currentSessionLazy.Value;
            }
        }

#if DEBUG
        internal IEnumerable<BlogPost> DebugAllBlogPosts()
        {
            using (var session = this.documentStore.OpenSession())
            {
                return session.Query<BlogPost>().ToList();
            }
        }
#endif

        internal async Task Refresh(string blogKey, BlogData blogData, bool throwOnException = false)
        {
            var repositoryRefreshService = new RepositoryRefreshService(this, blogKey, throwOnException);
            await repositoryRefreshService.Refresh(blogData);
        }

        public async Task Refresh(string blogKey, DataSourceRefreshResult refreshResult, bool throwOnException = false)
        {
            var repositoryRefreshService = new RepositoryRefreshService(this, blogKey, throwOnException);
            await repositoryRefreshService.Refresh(refreshResult);
        }

        public bool WaitForData(params string[] blogKeys)
        {
            var timeout = TimeSpan.FromSeconds(WaitForDataTimeoutSeconds);
            using (var session = this.documentStore.OpenSession())
            {
                var query = session.Query<BlogRefresh>().Where(x => x.BlogKey.In(blogKeys));
                var waitFor = WaitForData(query, timeout);

                return waitFor.Any();
            }
        }

        public bool WaitForPosts(params string[] blogKeys)
        {
            var timeout = TimeSpan.FromSeconds(WaitForPostsTimeoutSeconds);
            using (var session = this.documentStore.OpenSession())
            {
                var query = session.Query<BlogPost>().FilterOnBlogKeys(blogKeys);
                var waitFor = WaitForData(query, timeout);

                return waitFor.Any();
            }
        }
        
        internal IRavenQueryable<BlogPost> GetBlogPostQuery(params string[] blogKeys)
        {
            return
                this.GetQuery<BlogPost, BlogPostsOrderedByCreated>()
                    .FilterOnBlogKeys(blogKeys)
                    .OrderByDescending(x => x.Published);
        }

        internal IAsyncDocumentSession GetMaxRequestSessionAsync()
        {
            var maximumSession = this.documentStore.OpenAsyncSession();
            maximumSession.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

            return maximumSession;
        }

        internal IDocumentSession GetMaxRequestSession()
        {
            var maximumSession = this.documentStore.OpenSession();
            maximumSession.Advanced.MaxNumberOfRequestsPerSession = int.MaxValue;

            return maximumSession;
        }

        internal IRavenQueryable<BlogPost> GetMaxRequestQuery<TIndexCreator>(params string[] blogKeys)
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            using (var maximumSession = this.GetMaxRequestSession())
            {
                return maximumSession.Query<BlogPost, TIndexCreator>().FilterOnBlogKeys(blogKeys);
            }
        }

        internal IRavenQueryable<T> GetQuery<T, TIndexCreator>()
            where TIndexCreator : AbstractIndexCreationTask, new()
        {
            return this.CurrentSession.Query<T, TIndexCreator>();
        }

        private static IRavenQueryable<T> WaitForData<T>(IRavenQueryable<T> query, TimeSpan timeout)
        {
            return query.Customize(x => x.WaitForNonStaleResults(timeout));
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (this.currentSessionLazy.IsValueCreated && this.currentSessionLazy.Value != null)
            {
                this.currentSessionLazy.Value.Dispose();
            }
        }

        #endregion
    }
}
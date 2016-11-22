using System;
using System.Collections.Generic;
using System.Linq;

using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Listeners;

namespace Blaven.DataStorage.RavenDb.Tests
{
    public static class EmbeddableDocumentStoreHelper
    {
        private static readonly object RavenDbInitializerLock = new object();

        public static EmbeddableDocumentStore Get(string path = null, bool initIndexes = true)
        {
            path = !string.IsNullOrWhiteSpace(path) ? path : $"{Guid.NewGuid()}";

            var documentStore = new EmbeddableDocumentStore
                                    {
                                        Configuration =
                                            {
                                                DataDirectory = path,
                                                RunInMemory = true,
                                                RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                                                MaxPageSize = 1024
                                            },
                                        RunInMemory = true
                                    };

            if (initIndexes)
            {
                lock (RavenDbInitializerLock)
                {
                    RavenDbInitializer.Initialize(documentStore).GetAwaiter().GetResult();
                }
            }

            documentStore.RegisterListener(new WaitForNonStaleResultsAsOfNowQueryListener());

            return documentStore;
        }

        public static EmbeddableDocumentStore GetWithData(
            string path = null,
            bool initIndexes = true,
            IEnumerable<BlogMeta> blogMetas = null,
            IEnumerable<BlogPost> blogPosts = null)
        {
            var documentStore = Get(path, initIndexes);

            var blogMetaList = blogMetas?.ToList() ?? new List<BlogMeta>(0);
            var blogPostList = blogPosts?.ToList() ?? new List<BlogPost>(0);

            if (blogMetaList.Any() || blogPostList.Any())
            {
                using (var session = documentStore.OpenSession())
                {
                    blogMetaList.ForEach(x => session.Store(x));
                    blogPostList.ForEach(x => session.Store(x));

                    session.SaveChanges();
                }
            }

            return documentStore;
        }

        private class WaitForNonStaleResultsAsOfNowQueryListener : IDocumentQueryListener
        {
            public void BeforeQueryExecuted(IDocumentQueryCustomization customization)
            {
                customization.WaitForNonStaleResultsAsOfNow();
            }
        }
    }
}
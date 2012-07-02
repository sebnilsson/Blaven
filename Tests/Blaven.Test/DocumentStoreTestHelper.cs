using System;

using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Blaven.Test {
    public static class DocumentStoreTestHelper {
        public static EmbeddableDocumentStore GetEmbeddableDocumentStore() {
            string randomPath = new Random().Next().ToString();
            return GetEmbeddableDocumentStore(randomPath);
        }

        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(string path) {
            var documentStore = new EmbeddableDocumentStore {
                Configuration = {
                    DataDirectory = path,
                    RunInUnreliableYetFastModeThatIsNotSuitableForProduction = true,
                    DefaultStorageTypeName = "munin",
                    RunInMemory = true,
                },
                RunInMemory = true,
            };
            documentStore.Initialize();

            IndexCreation.CreateIndexes(
                typeof(Blaven.RavenDb.Indexes.BlogPostsOrderedByCreated).Assembly, documentStore);            

            return documentStore;
        }

        public static void WaitForIndexes(IDocumentStore documentStore) {
            while(documentStore.DatabaseCommands.GetStatistics().StaleIndexes.Length > 0) {
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}

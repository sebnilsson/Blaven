using System;

using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Blaven.Test {
    public static class DocumentStoreTestHelper {
        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(bool createIndexes = false) {
            string randomPath = new Random().Next().ToString();
            return GetEmbeddableDocumentStore(randomPath, createIndexes);
        }

        public static EmbeddableDocumentStore GetEmbeddableDocumentStore(string path, bool createIndexes = false) {
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

            if(createIndexes) {
                IndexCreation.CreateIndexes(
                    typeof(Blaven.RavenDb.Indexes.BlogPostsOrderedByCreated).Assembly, documentStore);
            }         

            return documentStore;
        }
    }
}

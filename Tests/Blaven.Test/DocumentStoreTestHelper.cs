using System;

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

            return documentStore;
        }
    }
}

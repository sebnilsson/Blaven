using System;

using Blaven.RavenDb;
using Raven.Client.Embedded;

namespace Blaven.Test
{
    public static class DocumentStoreTestHelper
    {
        public static EmbeddableDocumentStore GetEmbeddableDocumentStore()
        {
            var documentStore = GetEmbeddableDocumentStore(Guid.NewGuid().ToString());

            RavenDbHelper.InitWithIndexes(documentStore);

            return documentStore;
        }

        private static EmbeddableDocumentStore GetEmbeddableDocumentStore(string path)
        {
            var documentStore = new EmbeddableDocumentStore
                {
                    Configuration =
                        {
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
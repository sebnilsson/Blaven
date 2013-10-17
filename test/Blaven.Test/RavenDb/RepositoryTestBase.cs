using Blaven.Test;

namespace Blaven.RavenDb.Test
{
    public class RepositoryTestBase : BlavenTestBase
    {
        internal Repository GetRepository()
        {
            var documentStore = GetEmbeddableDocumentStore();
            return new Repository(documentStore);
        }
    }
}
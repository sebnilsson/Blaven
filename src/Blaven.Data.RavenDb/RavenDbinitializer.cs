using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Blaven.Data.RavenDb.Indexes;

using Raven.Client;
using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb
{
    public static class RavenDbInitializer
    {
        public static async Task Initialize(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            documentStore.Initialize();

            RavenDbIdConventions.Init(documentStore);

#if (NET45 || NET451 || NET452 || NET46 || NET461 || NET46)
            var assembly = Assembly.GetAssembly(typeof(BlogPostsIndex));
#else
            var assembly = typeof(BlogPostsIndex).GetTypeInfo().Assembly;
#endif

            await IndexCreation.CreateIndexesAsync(assembly, documentStore);
        }
    }
}
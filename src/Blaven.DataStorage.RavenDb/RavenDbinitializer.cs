using System;
using System.Reflection;
using System.Threading.Tasks;

using Blaven.DataStorage.RavenDb.Indexes;

using Raven.Client;
using Raven.Client.Indexes;

namespace Blaven.DataStorage.RavenDb
{
    public static class RavenDbInitializer
    {
        public static async Task Initialize(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            try
            {
                await InitializeInternal(documentStore);
            }
            catch (Exception ex)
            {
                throw new RavenDbInitializerException($"Failed to initialize RavenDB: {ex.Message}", ex);
            }
        }

        private static async Task InitializeInternal(IDocumentStore documentStore)
        {
            documentStore.Initialize();

            RavenDbIdConventions.Init(documentStore);

            var assembly = typeof(BlogPostsIndex).GetTypeInfo().Assembly;

            await IndexCreation.CreateIndexesAsync(assembly, documentStore);
        }
    }
}
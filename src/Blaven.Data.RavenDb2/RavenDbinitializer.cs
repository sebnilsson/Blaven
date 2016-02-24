using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Blaven.Data.RavenDb2.Indexes;
using Raven.Client;
using Raven.Client.Indexes;

namespace Blaven.Data.RavenDb2
{
    public static class RavenDbInitializer
    {
        public static void Initialize(IDocumentStore documentStore)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }

            documentStore.Initialize();

            RavenDbIdConventions.Init(documentStore);

            InitIndexes(documentStore);
        }

        private static void InitIndexes(IDocumentStore documentStore)
        {
            var existingIndexes = documentStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);

            var blavenIndexNames = GetBlavenIndexNames();
            var hasAllIndexes = blavenIndexNames.All(existingIndexes.Contains);

            var createIndexesTask =
                Task.Factory.StartNew(
                    () => IndexCreation.CreateIndexes(typeof(BlogPostsIndex).Assembly, documentStore));

            if (!hasAllIndexes)
            {
                createIndexesTask.Wait();
            }
        }

        private static IEnumerable<string> GetBlavenIndexNames()
        {
            try
            {
                var blavenIndexNames =
                    Assembly.GetAssembly(typeof(RavenDbInitializer))
                        .GetTypes()
                        .Where(x => x.IsSubclassOf(typeof(AbstractIndexCreationTask)))
                        .Select(x => x.Name);
                return blavenIndexNames;
            }
            catch (ReflectionTypeLoadException ex)
            {
                var firstLoaderExceptionMessage = ex.LoaderExceptions?.FirstOrDefault()?.Message.TrimEnd('.');

                string message = $"Failed to find RavenDB-indexes in assembly: {firstLoaderExceptionMessage}.";
                throw new RavenDbInitializerException(message, ex);
            }
        }
    }
}
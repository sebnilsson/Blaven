using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Blaven.RavenDb.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Blaven.RavenDb
{
    public static class RavenDbHelper
    {
        /// <summary>
        /// Gets an instance of a DocumentStore, using the values in AppSettings for URL and API-key.
        /// </summary>
        /// <returns></returns>
        public static DocumentStore GetDefaultDocumentStore(bool initStore = true)
        {
            var documentStore = new DocumentStore
                                    {
                                        ApiKey = AppSettingsService.RavenDbStoreApiKey,
                                        Url = AppSettingsService.RavenDbStoreUrl,
                                    };

            if (initStore)
            {
                InitWithIndexes(documentStore);
            }

            return documentStore;
        }

        public static string GetEntityId<T>(params object[] ids)
        {
            if (ids == null || !ids.Any())
            {
                throw new ArgumentOutOfRangeException("ids", "The provided keys cannot be null or empty.");
            }

            string typeName = typeof(T).Name.ToLowerInvariant();
            string key = String.Join("/", new[] { typeName }.Concat(ids));
            return key;
        }

        /// <summary>
        /// Initializes the given document store and creates needed indexes for Blaven.
        /// </summary>
        /// <param name="documentStore">The document to initialize and create indexes for.</param>
        public static void InitWithIndexes(IDocumentStore documentStore)
        {
            documentStore.Initialize();

            documentStore.Conventions.MaxNumberOfRequestsPerSession = 100;

            var existingIndexes = documentStore.DatabaseCommands.GetIndexNames(0, Int32.MaxValue);
            IEnumerable<string> blavenIndexes;
            try
            {
                blavenIndexes =
                    Assembly.GetAssembly(typeof(BlogService))
                            .GetTypes()
                            .Where(x => x.IsSubclassOf(typeof(AbstractIndexCreationTask)))
                            .Select(x => x.Name);
            }
            catch (ReflectionTypeLoadException ex)
            {
                var firstError = ex.LoaderExceptions.FirstOrDefault();
                string message = (firstError != null) ? firstError.Message : String.Empty;

                throw new BlavenException(ex, message);
            }
            var hasAllIndexes = blavenIndexes.All(existingIndexes.Contains);

            var createIndexesTask =
                new Task(
                    () =>
                    IndexCreation.CreateIndexes(
                        typeof(BlogPostsOrderedByCreated).Assembly, documentStore));
            createIndexesTask.Start();

            if (!hasAllIndexes)
            {
                createIndexesTask.Wait();
            }
        }

        public static void HandleRavenExceptions(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                if (ex.Source == "Raven.Database")
                {
                    throw new RavenDbNotInitException(ex);
                }
                throw;
            }
        }

        public static T HandleRavenExceptions<T>(Func<T> function)
        {
            try
            {
                return function();
            }
            catch (Exception ex)
            {
                if (ex.Source == "Raven.Database")
                {
                    throw new RavenDbNotInitException(ex);
                }
                throw;
            }
        }
    }
}
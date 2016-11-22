using System;

using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.DataStorage.RavenDb.Tests
{
    public static class DocumentStoreExtensions
    {
        public static TResult QueryNonStale<T, TResult>(
            this IDocumentStore documentStore,
            Func<IRavenQueryable<T>, TResult> queryFactory)
        {
            if (documentStore == null)
            {
                throw new ArgumentNullException(nameof(documentStore));
            }
            if (queryFactory == null)
            {
                throw new ArgumentNullException(nameof(queryFactory));
            }

            TResult result;

            using (var session = documentStore.OpenSession())
            {
                var query = session.QueryNonStale<T>();

                result = queryFactory(query);
            }

            return result;
        }
    }
}
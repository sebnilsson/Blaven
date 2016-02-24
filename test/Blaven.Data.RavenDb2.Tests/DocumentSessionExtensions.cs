using System;

using Raven.Client;
using Raven.Client.Linq;

namespace Blaven.Data.RavenDb2.Tests
{
    public static class DocumentSessionExtensions
    {
        public static IRavenQueryable<T> QueryNonStale<T>(this IDocumentSession documentSession)
        {
            if (documentSession == null)
            {
                throw new ArgumentNullException(nameof(documentSession));
            }

            var query = documentSession.Query<T>().Customize(x => x.WaitForNonStaleResultsAsOfNow());
            return query;
        }
    }
}
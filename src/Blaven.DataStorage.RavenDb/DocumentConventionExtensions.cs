using System;
using System.Threading.Tasks;

using Raven.Client.Document;

namespace Blaven.DataStorage.RavenDb
{
    public static class DocumentConventionExtensions
    {
        public static DocumentConvention RegisterIdConventions<TEntity>(
            this DocumentConvention conventions,
            Func<TEntity, string> idFactory)
        {
            if (conventions == null)
            {
                throw new ArgumentNullException(nameof(conventions));
            }
            if (idFactory == null)
            {
                throw new ArgumentNullException(nameof(idFactory));
            }

            conventions.RegisterIdConvention<TEntity>((dbName, commands, entity) => idFactory(entity));
            conventions.RegisterAsyncIdConvention<TEntity>(
                async (dbName, commands, entity) => await Task.FromResult(idFactory(entity)));

            return conventions;
        }
    }
}
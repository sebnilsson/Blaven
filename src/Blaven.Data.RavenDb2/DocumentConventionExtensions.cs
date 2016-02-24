﻿using System;
using System.Threading.Tasks;

using Raven.Client.Document;

namespace Blaven.Data.RavenDb2
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
                (dbName, commands, entity) => Task.FromResult(idFactory(entity)));

            return conventions;
        }
    }
}
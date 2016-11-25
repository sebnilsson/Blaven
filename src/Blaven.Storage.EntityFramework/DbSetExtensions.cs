using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Blaven.DataStorage.EntityFramework
{
    public static class DbSetExtensions
    {
        public static async Task<TEntity> SingleOrDefaultLocalOrSourceAsync<TEntity>(
            this DbSet<TEntity> dbSet,
            Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            if (dbSet == null)
            {
                throw new ArgumentNullException(nameof(dbSet));
            }
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var predicateFunc = predicate.Compile();

            var localEntity = dbSet.Local?.SingleOrDefault(predicateFunc);
            if (localEntity != null)
            {
                return localEntity;
            }

            var entity = await dbSet.SingleOrDefaultAsync(predicate);
            return entity;
        }
    }
}
using System;
using System.Linq;

namespace Blaven.Storage
{
    public class BlogPostQuery
    {
        public BlogPostQuery(
            IQueryable<BlogPost> queryable,
            BlogQueryOptions options)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            Options = options;

            Query = GetQuery(queryable);
        }

        public BlogQueryOptions Options { get; }

        public IQueryable<BlogPost> Query { get; }

        private IQueryable<BlogPost> GetQuery(IQueryable<BlogPost> queryable)
        {
            var query = queryable;

            if (!Options.IncludeDrafts)
            {
                query = query.Where(x => !x.IsDraft);
            }

            return query;
        }
    }
}

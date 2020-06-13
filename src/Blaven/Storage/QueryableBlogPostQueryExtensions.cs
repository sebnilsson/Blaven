using System;
using System.Linq;

namespace Blaven.Storage
{
    public static class QueryableBlogPostQueryExtensions
    {
        public static BlogPostQuery AsBlogPostQuery(
            this IQueryable<BlogPost> queryable,
            BlogQueryOptions options)
        {
            if (queryable is null)
                throw new ArgumentNullException(nameof(queryable));
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            return new BlogPostQuery(queryable, options);
        }
    }
}

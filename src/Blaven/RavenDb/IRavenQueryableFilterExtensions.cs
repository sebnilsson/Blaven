using System;

using Raven.Client.Linq;

namespace Blaven.RavenDb
{
    public static class IRavenQueryableFilterExtensions
    {
        public static IRavenQueryable<BlogPost> FilterOnBlogKeys(
            this IRavenQueryable<BlogPost> query, params string[] blogKeys)
        {
            if (blogKeys == null)
            {
                throw new ArgumentNullException("blogKeys");
            }

            return query.Where(x => x.BlogKey.In(blogKeys));
        }
    }
}
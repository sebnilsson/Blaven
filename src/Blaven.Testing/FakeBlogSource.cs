using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSources;
using Blaven.Queries;

namespace Blaven.Testing
{
    public class FakeBlogSource : IBlogSource
    {
        private readonly List<BlogMeta> _metas;
        private readonly List<BlogPost> _posts;

        internal IQueryable<BlogMeta> Metas => _metas.AsQueryable();
        internal IQueryable<BlogPost> Posts => _posts.AsQueryable();

        public FakeBlogSource(
            IEnumerable<BlogPost>? posts = null,
            IEnumerable<BlogMeta>? metas = null)
        {
            _posts = (posts ?? Enumerable.Empty<BlogPost>()).ToList();
            _metas = (metas ?? Enumerable.Empty<BlogMeta>()).ToList();
        }

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var meta =
                Metas
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .FirstOrDefault();

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<IReadOnlyList<BlogPost>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var posts =
                Posts
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .ToList()
                    as IReadOnlyList<BlogPost>;

            return Task.FromResult(posts);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorage : IInMemoryStorage
    {
        private readonly ICollection<BlogMeta> _metas = new HashSet<BlogMeta>();
        private readonly ICollection<BlogPost> _posts = new HashSet<BlogPost>();

        public InMemoryStorage()
            : this(
                Enumerable.Empty<BlogMeta>(),
                Enumerable.Empty<BlogPost>())
        {
        }

        public InMemoryStorage(
            IEnumerable<BlogMeta> metas,
            IEnumerable<BlogPost> posts)
        {
            _metas = (metas ?? Enumerable.Empty<BlogMeta>()).ToHashSet();
            _posts = (posts ?? Enumerable.Empty<BlogPost>()).ToHashSet();
        }

        public IQueryable<BlogMeta> Metas => _metas.AsQueryable();

        public IQueryable<BlogPost> Posts => _posts.AsQueryable();

        public void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta)
        {
            var existingMetas =
                _metas.Where(x => x.BlogKey == blogKey).ToList();

            existingMetas.ForEach(x => _metas.Remove(x));

            if (meta is null)
            {
                return;
            }

            _metas.Add(meta);
        }

        public void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            RemovePosts(blogKey, post.Id);

            _posts.Add(post);
        }

        public void RemovePosts(BlogKey blogKey, string? id = null)
        {
            var existingPosts =
                _posts
                    .Where(x => x.BlogKey == blogKey)
                    .Where(x => string.IsNullOrWhiteSpace(id) || x.Id == id)
                    .ToList();

            existingPosts.ForEach(x => _posts.Remove(x));
        }
    }
}

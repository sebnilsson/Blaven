using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorage : IInMemoryStorage
    {
        private readonly ICollection<BlogMeta> _metas = new List<BlogMeta>();
        private readonly ICollection<BlogPost> _posts = new List<BlogPost>();

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

        public IQueryable<BlogMeta> Metas => GetMetaQuery();

        public IQueryable<BlogPost> Posts => GetPostsQuery();

        private IQueryable<BlogMeta> GetMetaQuery()
        {
            lock (_metas)
            {
                return _metas.AsQueryable();
            }
        }

        private IQueryable<BlogPost> GetPostsQuery()
        {
            lock (_posts)
            {
                return _posts.AsQueryable();
            }
        }

        public void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta)
        {
            lock (_metas)
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
        }

        public void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            lock (_posts)
            {
                RemovePostsInternal(blogKey, post.Id);

                _posts.Add(post);
            }
        }

        public void RemovePosts(BlogKey blogKey, string? id = null)
        {
            lock (_posts)
            {
                RemovePostsInternal(blogKey, id);
            }
        }

        private void RemovePostsInternal(BlogKey blogKey, string? id)
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

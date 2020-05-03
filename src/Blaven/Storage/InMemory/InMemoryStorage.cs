using System.Collections.Generic;
using System.Linq;

namespace Blaven.Storage.InMemory
{
    public class InMemoryStorage : IInMemoryStorage
    {
        private readonly List<BlogMeta> _metas = new List<BlogMeta>();
        private readonly List<BlogPost> _posts = new List<BlogPost>();

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
            _metas = (metas ?? Enumerable.Empty<BlogMeta>()).ToList();
            _posts = (posts ?? Enumerable.Empty<BlogPost>()).ToList();
        }

        public IQueryable<BlogMeta> Metas => _metas.AsQueryable();

        public IQueryable<BlogPost> Posts => _posts.AsQueryable();

        public void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta)
        {
            if (meta is null)
            {
                return;
            }

            _metas.RemoveAll(x => x.BlogKey == blogKey);

            _metas.Add(meta);
        }

        public void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            RemovePosts(blogKey, post.Id);

            _posts.Add(post);
        }

        public void RemovePosts(BlogKey blogKey)
        {
            _posts.RemoveAll(x => x.BlogKey == blogKey);
        }

        public void RemovePosts(BlogKey blogKey, string id)
        {
            _posts.RemoveAll(x => x.BlogKey == blogKey && x.Id == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blaven.Storage
{
    public class InMemoryStorage : IStorage
    {
        private readonly List<BlogMeta> _metas = new List<BlogMeta>();
        private readonly List<BlogPost> _posts = new List<BlogPost>();

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var meta =
                _metas
                    .FirstOrDefault(x =>
                        x.BlogKey == blogKey
                        && x.UpdatedAt > lastUpdatedAt);

            return Task.FromResult<BlogMeta?>(meta);
        }

        public Task<IReadOnlyList<BlogPostBase>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? lastUpdatedAt)
        {
            var posts =
                _posts
                    .Where(x =>
                        x.BlogKey == blogKey
                        && x.UpdatedAt > lastUpdatedAt)
                    .ToList();

            return Task.FromResult(posts as IReadOnlyList<BlogPostBase>);
        }

        public Task Update(
            BlogKey blogKey,
            BlogMeta? meta,
            IEnumerable<BlogPost> insertedPosts,
            IEnumerable<BlogPost> updatedPosts,
            IEnumerable<BlogPostBase> deletedPosts,
            DateTimeOffset? lastUpdatedAt)
        {
            if (lastUpdatedAt == null)
            {
                _posts.RemoveAll(x => x.BlogKey == blogKey);
            }

            CreateOrUpdateMeta(blogKey, meta);

            foreach (var post in insertedPosts)
            {
                CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in updatedPosts)
            {
                CreateOrUpdatePost(blogKey, post);
            }

            foreach (var post in deletedPosts)
            {
                DeletePost(blogKey, post);
            }

            return Task.CompletedTask;
        }

        private void CreateOrUpdateMeta(BlogKey blogKey, BlogMeta? meta)
        {
            if (meta is null)
            {
                return;
            }

            _metas.RemoveAll(x => x.BlogKey == blogKey);

            _metas.Add(meta);
        }

        private void CreateOrUpdatePost(BlogKey blogKey, BlogPost post)
        {
            DeletePost(blogKey, post);

            _posts.Add(post);
        }

        private void DeletePost(BlogKey blogKey, BlogPostBase post)
        {
            _posts.RemoveAll(x => x.BlogKey == blogKey && x.Id == post.Id);
        }
    }
}

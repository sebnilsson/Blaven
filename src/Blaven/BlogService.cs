using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.DataStorage;

namespace Blaven
{
    public class BlogService
    {
        private readonly BlogSettingsHelper _blogSettings;
        private readonly IRepository _repository;

        public BlogService(IRepository repository, params BlogSetting[] blogSettings) : this(repository,
            blogSettings?.AsEnumerable())
        {
        }

        public BlogService(IRepository repository, IEnumerable<BlogSetting> blogSettings)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _blogSettings = new BlogSettingsHelper(blogSettings ?? Enumerable.Empty<BlogSetting>());
        }

        public async Task<BlogMeta> GetBlogMeta(BlogKey blogKey = null)
        {
            var ensuredBlogKey = _blogSettings.GetEnsuredBlogKey(blogKey);

            var meta = await _repository.GetBlogMeta(ensuredBlogKey);
            return meta;
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            var meta = _repository.GetBlogMetas().OrderBy(x => x.Name);
            return meta;
        }

        public async Task<BlogPost> GetPost(string blavenId, BlogKey blogKey = null)
        {
            if (blavenId == null)
                throw new ArgumentNullException(nameof(blavenId));

            var ensuredBlogKey = _blogSettings.GetEnsuredBlogKey(blogKey);

            var post = await _repository.GetPost(ensuredBlogKey, blavenId);
            return post;
        }

        public async Task<BlogPost> GetPostBySourceId(string sourceId, BlogKey blogKey = null)
        {
            if (sourceId == null)
                throw new ArgumentNullException(nameof(sourceId));

            var ensuredBlogKey = _blogSettings.GetEnsuredBlogKey(blogKey);

            var post = await _repository.GetPostBySourceId(ensuredBlogKey, sourceId);
            return post;
        }

        public IQueryable<BlogArchiveItem> ListArchive(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allArchive = _repository.ListArchive(ensuredBlogKeys).OrderByDescending(x => x.Date);
            return allArchive;
        }

        public IQueryable<BlogPostHead> ListPostHeads(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var postHeads = _repository.ListPostHeads(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return postHeads;
        }

        public IQueryable<BlogPost> ListPosts(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = _repository.ListPosts(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByArchive(DateTime archiveDate, params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = _repository.ListPostsByArchive(ensuredBlogKeys, archiveDate)
                .OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByTag(string tagName, params BlogKey[] blogKeys)
        {
            if (tagName == null)
                throw new ArgumentNullException(nameof(tagName));

            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = _repository.ListPostsByTag(ensuredBlogKeys, tagName).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogTagItem> ListTags(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allTags = _repository.ListTags(ensuredBlogKeys).OrderBy(x => x.Name);
            return allTags;
        }

        public IQueryable<BlogPost> Search(string search, params BlogKey[] blogKeys)
        {
            if (search == null)
                throw new ArgumentNullException(nameof(search));

            var ensuredBlogKeys = _blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = _repository.SearchPosts(ensuredBlogKeys, search).OrderByDescending(x => x.PublishedAt);
            return posts;
        }
    }
}
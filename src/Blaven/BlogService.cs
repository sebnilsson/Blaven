using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Blaven.DataStorage;

namespace Blaven
{
    public class BlogService
    {
        private readonly BlogSettingsHelper blogSettings;

        private readonly IRepository repository;

        public BlogService(IRepository repository, params BlogSetting[] blogSettings)
            : this(repository, blogSettings?.AsEnumerable())
        {
        }

        public BlogService(IRepository repository, IEnumerable<BlogSetting> blogSettings)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;

            this.blogSettings = new BlogSettingsHelper(blogSettings ?? Enumerable.Empty<BlogSetting>());
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            var meta = this.repository.GetBlogMetas().OrderBy(x => x.Name);
            return meta;
        }

        public async Task<BlogMeta> GetBlogMeta(BlogKey blogKey = null)
        {
            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var meta = await this.repository.GetBlogMeta(ensuredBlogKey);
            return meta;
        }

        public async Task<BlogPost> GetPost(string blavenId, BlogKey blogKey = null)
        {
            if (blavenId == null)
            {
                throw new ArgumentNullException(nameof(blavenId));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var post = await this.repository.GetPost(ensuredBlogKey, blavenId);
            return post;
        }

        public async Task<BlogPost> GetPostBySourceId(string sourceId, BlogKey blogKey = null)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var post = await this.repository.GetPostBySourceId(ensuredBlogKey, sourceId);
            return post;
        }

        public IQueryable<BlogArchiveItem> ListArchive(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allArchive = this.repository.ListArchive(ensuredBlogKeys).OrderByDescending(x => x.Date);
            return allArchive;
        }

        public IQueryable<BlogTagItem> ListTags(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allTags = this.repository.ListTags(ensuredBlogKeys).OrderBy(x => x.Name);
            return allTags;
        }

        public IQueryable<BlogPostHead> ListPostHeads(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var postHeads = this.repository.ListPostHeads(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return postHeads;
        }

        public IQueryable<BlogPost> ListPosts(params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = this.repository.ListPosts(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByArchive(DateTime archiveDate, params BlogKey[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts =
                this.repository.ListPostsByArchive(ensuredBlogKeys, archiveDate).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByTag(string tagName, params BlogKey[] blogKeys)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = this.repository.ListPostsByTag(ensuredBlogKeys, tagName).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> Search(string search, params BlogKey[] blogKeys)
        {
            if (search == null)
            {
                throw new ArgumentNullException(nameof(search));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = this.repository.SearchPosts(ensuredBlogKeys, search).OrderByDescending(x => x.PublishedAt);
            return posts;
        }
    }
}
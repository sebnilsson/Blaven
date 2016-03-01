﻿using System;
using System.Linq;

using Blaven.Data;

namespace Blaven
{
    public class BlogQueryService
    {
        private readonly BlogSettingsManager blogSettings;

        private readonly IRepository repository;

        public BlogQueryService(IRepository repository, params BlogSetting[] blogSettings)
        {
            if (repository == null)
            {
                throw new ArgumentNullException(nameof(repository));
            }

            this.repository = repository;

            this.blogSettings = new BlogSettingsManager(blogSettings ?? Enumerable.Empty<BlogSetting>());
        }

        public IQueryable<BlogMeta> GetBlogMetas()
        {
            var meta = this.repository.GetBlogMetas().OrderBy(x => x.Name);
            return meta;
        }

        public BlogMeta GetBlogMeta(string blogKey = null)
        {
            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var meta = this.repository.GetBlogMeta(ensuredBlogKey);
            return meta;
        }

        public BlogPost GetPost(string blavenId, string blogKey = null)
        {
            if (blavenId == null)
            {
                throw new ArgumentNullException(nameof(blavenId));
            }

            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var post = this.repository.GetPost(ensuredBlogKey, blavenId);
            return post;
        }

        public BlogPost GetPostBySourceId(string sourceId, string blogKey = null)
        {
            var ensuredBlogKey = this.blogSettings.GetEnsuredBlogKey(blogKey);

            var post = this.repository.GetPostBySourceId(ensuredBlogKey, sourceId);
            return post;
        }

        public IQueryable<BlogArchiveItem> ListArchive(params string[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allArchive = this.repository.ListArchive(ensuredBlogKeys).OrderByDescending(x => x.Date);
            return allArchive;
        }

        public IQueryable<BlogTagItem> ListTags(params string[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var allTags = this.repository.ListTags(ensuredBlogKeys).OrderBy(x => x.Name);
            return allTags;
        }

        public IQueryable<BlogPostHead> ListPostHeads(params string[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var postHeads = this.repository.ListPostHeads(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return postHeads;
        }

        public IQueryable<BlogPost> ListPosts(params string[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = this.repository.ListPosts(ensuredBlogKeys).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByArchive(DateTime archiveDate, params string[] blogKeys)
        {
            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts =
                this.repository.ListPostsByArchive(ensuredBlogKeys, archiveDate).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> ListPostsByTag(string tagName, params string[] blogKeys)
        {
            if (tagName == null)
            {
                throw new ArgumentNullException(nameof(tagName));
            }

            var ensuredBlogKeys = this.blogSettings.GetEnsuredBlogKeys(blogKeys);

            var posts = this.repository.ListPostsByTag(ensuredBlogKeys, tagName).OrderByDescending(x => x.PublishedAt);
            return posts;
        }

        public IQueryable<BlogPost> Search(string search, params string[] blogKeys)
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
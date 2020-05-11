using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.BlogSources.FileProviders;
using Blaven.Queries;

namespace Blaven.BlogSources.Markdown
{
    public class MarkdownBlogSource : IBlogSource
    {
        private readonly IFileDataProvider _fileDataProvider;

        // TODO: Refactor to allow composition and single call to file-provider
        public MarkdownBlogSource(IFileDataProvider fileDataProvider)
        {
            _fileDataProvider = fileDataProvider
                ?? throw new ArgumentNullException(nameof(fileDataProvider));
        }

        public async Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var fileData =
                await _fileDataProvider.GetFileData().ConfigureAwait(false);

            var metas = GetMetaQuery(fileData);

            return
                metas
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .FirstOrDefault();
        }

        public async Task<IReadOnlyList<BlogPost>> GetPosts(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var fileData =
                await _fileDataProvider.GetFileData().ConfigureAwait(false);

            // TODO: Fix double-fetching
            var posts = GetPostQuery(fileData);

            return
                posts
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .ToList();
        }

        private IQueryable<BlogMeta> GetJsonMetas(
            IEnumerable<FileData> metaJsonFiles)
        {
            return GetJsonMetasInternal(metaJsonFiles).ToList().AsQueryable();
        }

        private IEnumerable<BlogMeta> GetJsonMetasInternal(
            IEnumerable<FileData> metaJsonFiles)
        {
            foreach (var file in metaJsonFiles)
            {
                var meta = BlogMetaJsonParser.Parse(file);
                if (meta != null)
                {
                    yield return meta;
                }
            }
        }

        private IQueryable<BlogPost> GetMarkdownPosts(
            IEnumerable<FileData> postMarkdownFiles)
        {
            var markdownPosts =
                postMarkdownFiles
                    .Select(x => BlogPostMarkdownParser.Parse(x))
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();

            return markdownPosts.AsQueryable();
            //return
            //    GetMarkdownPostsInternal(postMarkdownFiles)
            //        .ToList()
            //        .AsQueryable();
        }

        private IEnumerable<BlogPost> GetMarkdownPostsInternal(
            IEnumerable<FileData> postMarkdownFiles)
        {
            foreach (var file in postMarkdownFiles)
            {
                var post = BlogPostMarkdownParser.Parse(file);
                if (post != null)
                {
                    yield return post;
                }
            }
        }

        private IQueryable<BlogMeta> GetMetaQuery(
            FileDataResult fileData)
        {
            //var fileData = await _fileData.Value.ConfigureAwait(false);

            return GetJsonMetas(fileData.Metas);
        }

        private IQueryable<BlogPost> GetPostQuery(
            FileDataResult fileData)
        {
            //var fileData = await _fileData.Value.ConfigureAwait(false);

            return GetMarkdownPosts(fileData.Posts);
        }
    }
}

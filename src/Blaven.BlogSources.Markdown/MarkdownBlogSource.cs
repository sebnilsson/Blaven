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
        private readonly BlogPostMarkdownParser _blogPostMarkdownParser =
            new BlogPostMarkdownParser();

        public MarkdownBlogSource(IFileDataProvider fileDataProvider)
        {
            _fileDataProvider = fileDataProvider
                ?? throw new ArgumentNullException(nameof(fileDataProvider));
        }

        public async Task<BlogSourceData> GetData(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var fileData =
                await _fileDataProvider.GetFileData().ConfigureAwait(false);

            var meta = GetJsonMeta(blogKey, updatedAfter, fileData);

            var posts = GetMarkdownPosts(blogKey, updatedAfter, fileData);

            return new BlogSourceData(blogKey, meta, posts);
        }

        private BlogMeta? GetJsonMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter,
            FileDataResult fileDataResult)
        {
            var metas =
                fileDataResult
                    .Metas
                    .Select(x => BlogMetaJsonParser.Parse(x))
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();

            return
                metas
                    .AsQueryable()
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .FirstOrDefault();
        }

        private IReadOnlyList<BlogPost> GetMarkdownPosts(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter,
            FileDataResult fileDataResult)
        {
            var markdownPosts =
                fileDataResult
                    .Posts
                    .Select(x => _blogPostMarkdownParser.Parse(x))
                    .Where(x => x != null)
                    .Select(x => x!)
                    .ToList();

            return
                markdownPosts
                    .AsQueryable()
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .ToList();
        }
    }
}

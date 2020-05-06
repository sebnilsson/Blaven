using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Queries;

namespace Blaven.BlogSources.Markdown
{
    public class MarkdownBlogSource : IBlogSource
    {
        private readonly IReadOnlyList<BlogMeta> _metas;
        private readonly IReadOnlyList<BlogPost> _posts;

        internal IQueryable<BlogMeta> Metas => _metas.AsQueryable();
        internal IQueryable<BlogPost> Posts => _posts.AsQueryable();

        public MarkdownBlogSource(
            IEnumerable<string> metaJsonFiles,
            IEnumerable<string> postMarkdownFiles)
            : this(
                metaJsonFiles: ToFileData(metaJsonFiles),
                postMarkdownFiles: ToFileData(postMarkdownFiles))
        {
        }

        public MarkdownBlogSource(
            IEnumerable<FileData> metaJsonFiles,
            IEnumerable<FileData> postMarkdownFiles)
        {
            _metas =
                GetJsonMetas(metaJsonFiles ?? Enumerable.Empty<FileData>())
                .ToList();

            _posts =
                GetMarkdownPosts(postMarkdownFiles ?? Enumerable.Empty<FileData>())
                .ToList();
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

        private IEnumerable<BlogMeta> GetJsonMetas(
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

        private IEnumerable<BlogPost> GetMarkdownPosts(
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

        private static IEnumerable<FileData> ToFileData(
            IEnumerable<string> source)
        {
            return
                source?.Select(x => new FileData(x))
                ?? Enumerable.Empty<FileData>();
        }
    }
}

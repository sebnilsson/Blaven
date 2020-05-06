using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blaven.Queries;

namespace Blaven.BlogSources.Markdown
{
    public class MarkdownBlogSource : IBlogSource
    {
        private readonly IReadOnlyList<BlogMeta> _blogMetas;
        private readonly IReadOnlyList<BlogPost> _blogPosts;

        internal IQueryable<BlogMeta> BlogMetas => _blogMetas.AsQueryable();
        internal IQueryable<BlogPost> BlogPosts => _blogPosts.AsQueryable();

        public MarkdownBlogSource(
            IEnumerable<string> metaJsons,
            IEnumerable<string> postMarkdowns)
            : this(
                metaJsons: ToTuples(metaJsons),
                postMarkdowns: ToTuples(postMarkdowns))
        {
        }

        public MarkdownBlogSource(
            IEnumerable<(string blogKey, string json)> metaJsons,
            IEnumerable<(string blogKey, string markdown)> postMarkdowns)
        {
            _blogMetas =
                GetJsonMetas(metaJsons ?? Enumerable.Empty<(string, string)>())
                .ToList();

            _blogPosts =
                GetMarkdownPosts(
                    postMarkdowns ?? Enumerable.Empty<(string, string)>())
                .ToList();
        }

        public Task<BlogMeta?> GetMeta(
            BlogKey blogKey,
            DateTimeOffset? updatedAfter = null)
        {
            var meta =
                BlogMetas
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
                BlogPosts
                    .WhereBlogKey(blogKey)
                    .WhereUpdatedAfter(updatedAfter)
                    .ToList()
                    as IReadOnlyList<BlogPost>;

            return Task.FromResult(posts);
        }

        private IEnumerable<BlogMeta> GetJsonMetas(
            IEnumerable<(string blogKey, string json)> metaJsons)
        {
            foreach (var (key, json) in metaJsons)
            {
                var blogKey = new BlogKey(key);

                var meta = BlogMetaJsonParser.Parse(blogKey, json);
                if (meta != null)
                {
                    yield return meta;
                }
            }
        }

        private IEnumerable<BlogPost> GetMarkdownPosts(
            IEnumerable<(string blogKey, string markdown)> blogPostFiles)
        {
            foreach (var (key, markdown) in blogPostFiles)
            {
                var blogKey = new BlogKey(key);

                var post = BlogPostMarkdownParser.Parse(blogKey, markdown);
                if (post != null)
                {
                    yield return post;
                }
            }
        }

        private static IEnumerable<(string, string)> ToTuples(
            IEnumerable<string> source)
        {
            return
                source?.Select(x => (string.Empty, x))
                ?? Enumerable.Empty<(string, string)>();
        }
    }
}

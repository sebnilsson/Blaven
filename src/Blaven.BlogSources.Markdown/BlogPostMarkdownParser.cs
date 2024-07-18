using System.IO;
using Blaven.BlogSources.FileProviders;

namespace Blaven.BlogSources.Markdown
{
    internal class BlogPostMarkdownParser
    {
        private readonly MarkdownConverter _markdownConverter = new();
        private readonly YamlConverter _yamlConverter = new();

        public BlogPost? Parse(FileData fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData.Content))
            {
                return null;
            }

            try
            {
                var post = ParseInternal(fileData);

                var fileName =
                    Path.GetFileNameWithoutExtension(fileData.FileName);

                post.BlogKey =
                    post.BlogKey.Value.Coalesce(fileData.KeyFolderName);

                post.Id = post.Id.Coalesce(fileName);
                post.Slug = post.Slug.Coalesce(fileName);
                post.SourceId ??= fileData.FileName;
                post.PublishedAt ??= fileData.CreatedAt;
                post.UpdatedAt ??= fileData.UpdatedAt;

                return post;
            }
            catch
            {
                return null;
            }
        }

        private BlogPost ParseInternal(FileData fileData)
        {
            var document = MarkdownDocumentParser.Parse(fileData.Content);

            var post = GetBlogPost(document);

            var html = _markdownConverter.ToHtml(document.Body);

            post.Content = html;

            return post;
        }

        private BlogPost GetBlogPost(MarkdownDocument document)
        {
            if (string.IsNullOrWhiteSpace(document.Yaml))
            {
                return new BlogPost();
            }

            try
            {
                return _yamlConverter.Deserialize<BlogPost>(document.Yaml);
            }
            catch
            {
                return new BlogPost();
            }
        }
    }
}

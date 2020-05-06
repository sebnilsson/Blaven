using Markdig;
using YamlDotNet.Serialization;

namespace Blaven.BlogSources.Markdown
{
    internal static class BlogPostMarkdownParser
    {
        private static readonly MarkdownPipeline s_markdownPipeline =
            new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        private static readonly IDeserializer s_yamlDeserializer =
            new DeserializerBuilder().IgnoreUnmatchedProperties().Build();

        public static BlogPost? Parse(FileData fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData.Content))
            {
                return null;
            }

            try
            {
                var post = ParseInternal(fileData);

                post.BlogKey =
                    post.BlogKey.HasValue
                    ? post.BlogKey
                    : fileData.FolderName;

                post.Id ??= fileData.FileName;
                post.Slug ??= fileData.FileName;
                post.SourceId ??= fileData.FileName;
                post.SourceUrl ??= fileData.FileName;

                // TODO: Consider making this into a Transform
                post.Hash ??= BlogPostHashUtility.GetHash(post);

                return post;
            }
            catch
            {
                return null;
            }
        }

        private static BlogPost ParseInternal(FileData fileData)
        {
            var document = MarkdownDocumentParser.Parse(fileData.Content);

            var post = GetBlogPost(document);

            var html =
                Markdig.Markdown.ToHtml(document.Body, s_markdownPipeline);

            post.Content = html;

            return post;
        }

        private static BlogPost GetBlogPost(MarkdownDocument document)
        {
            if (string.IsNullOrWhiteSpace(document.Yaml))
            {
                return new BlogPost();
            }

            try
            {
                return s_yamlDeserializer.Deserialize<BlogPost>(document.Yaml);
            }
            catch
            {
                return new BlogPost();
            }
        }
    }
}

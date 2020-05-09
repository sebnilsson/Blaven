namespace Blaven.BlogSources.Markdown
{
    internal static class BlogPostMarkdownParser
    {
        public static BlogPost? Parse(FileData fileData)
        {
            if (string.IsNullOrWhiteSpace(fileData.Content))
            {
                return null;
            }

            try
            {
                var post = ParseInternal(fileData);

                post.BlogKey = post.BlogKey.Value.Coalesce(fileData.FolderName);

                post.Id = post.Id.Coalesce(fileData.FileName);
                post.Slug = post.Slug.Coalesce(fileData.FileName);
                post.PublishedAt ??= fileData.CreatedAt;

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

            var html = MarkdownConverter.ToHtml(document.Body);

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
                return YamlConverter.Deserialize<BlogPost>(document.Yaml);
            }
            catch
            {
                return new BlogPost();
            }
        }
    }
}

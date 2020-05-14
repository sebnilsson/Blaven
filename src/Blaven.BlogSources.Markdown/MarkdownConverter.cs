using System;
using Ganss.XSS;
using Markdig;

namespace Blaven.BlogSources.Markdown
{
    internal class MarkdownConverter
    {
        private readonly HtmlSanitizer _htmlSanitizer =
            new HtmlSanitizer();

        private readonly MarkdownPipeline _markdownPipeline =
            new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

        public string ToHtml(string markdown)
        {
            if (markdown is null)
                throw new ArgumentNullException(nameof(markdown));

            var html = Markdig.Markdown.ToHtml(markdown, _markdownPipeline);

            var sanitizedHtml = _htmlSanitizer.Sanitize(html);
            return sanitizedHtml;
        }
    }
}

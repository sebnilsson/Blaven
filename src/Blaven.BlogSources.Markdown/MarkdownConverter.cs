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

            var sanitized = _htmlSanitizer.Sanitize(markdown);

            var html = Markdig.Markdown.ToHtml(sanitized, _markdownPipeline);
            return html;

            //var sanitizedHtml = s_htmlSanitizer.Sanitize(html);
            //return sanitizedHtml;
        }
    }
}

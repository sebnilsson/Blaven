using System;
using Ganss.XSS;
using Markdig;

namespace Blaven.BlogSources.Markdown
{
    internal static class MarkdownConverter
    {
        private static readonly HtmlSanitizer s_htmlSanitizer =
            new HtmlSanitizer();

        private static readonly MarkdownPipeline s_markdownPipeline =
            new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

        public static string ToHtml(string markdown)
        {
            if (markdown is null)
                throw new ArgumentNullException(nameof(markdown));

            var sanitized = s_htmlSanitizer.Sanitize(markdown);

            var html = Markdig.Markdown.ToHtml(sanitized, s_markdownPipeline);
            return html;

            //var sanitizedHtml = s_htmlSanitizer.Sanitize(html);
            //return sanitizedHtml;
        }
    }
}

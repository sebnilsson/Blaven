using System;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Blaven.Transformation.Extensions
{
    public class BlogPostSummaryTransform
        : IBlogPostQueryTransform, IBlogPostStorageTransform
    {
        private static readonly HtmlParser s_htmlParser = new HtmlParser();

        private readonly int _maxLength;
        private readonly string _suffix;

        public BlogPostSummaryTransform()
            : this(maxLength: 160, suffix: "...")
        {
        }

        public BlogPostSummaryTransform(int maxLength, string suffix)
        {
            _maxLength = maxLength;
            _suffix = suffix;
        }

        public void Transform(BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            TransformInternal(post);
        }

        public void Transform(BlogPostHeader postHeader)
        {
        }

        private void TransformInternal(BlogPost post)
        {
            if (!string.IsNullOrWhiteSpace(post.Summary)
                || string.IsNullOrWhiteSpace(post.Content))
            {
                return;
            }

            var summary = GetSummaryText(post);

            if (string.IsNullOrWhiteSpace(summary))
            {
                return;
            }

            post.Summary = summary;
        }

        private string GetSummaryText(BlogPost post)
        {
            var document = s_htmlParser.ParseDocument(post.Content);

            var paragraphTags = document.QuerySelectorAll("p");

            var sb = new StringBuilder();

            foreach (var paragraph in paragraphTags)
            {
                if (sb.Length > _maxLength)
                {
                    break;
                }

                var text = paragraph.Text();

                sb.Append($"{text} ");
            }

            var texts = sb.ToString().Trim();

            if (texts.Length > _maxLength)
            {
                texts = texts.Substring(0, _maxLength);
            }

            var lastSpace = texts.LastIndexOf(' ');

            var summary =
                (lastSpace > 1
                ? texts.Substring(0, lastSpace)
                : texts)
                .Trim();

            var isSentenceEnd = summary.EndsWith(".");

            return
                isSentenceEnd
                ? summary
                : $"{summary}{_suffix}";
        }
    }
}

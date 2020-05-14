using System;
using System.Web;
using AngleSharp.Html.Parser;

namespace Blaven.Transformation.Extensions
{
    public class BlogPostCodeTagDecodeTransform
        : IBlogPostQueryTransform, IBlogPostStorageTransform
    {
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
            if (string.IsNullOrWhiteSpace(post.Content))
            {
                return;
            }

            var decodedPreTagContent = GetDecodePreTagContent(post);

            post.Content = decodedPreTagContent;
        }

        private string GetDecodePreTagContent(BlogPost post)
        {
            var htmlParser = new HtmlParser();

            var document = htmlParser.ParseDocument(post.Content);

            var codeTags = document.QuerySelectorAll("code");

            foreach (var codeTag in codeTags)
            {
                var decodedHtml = HttpUtility.HtmlDecode(codeTag.InnerHtml);

                codeTag.InnerHtml = decodedHtml;
            }

            return document.Body?.InnerHtml ?? post.Content;
        }
    }
}

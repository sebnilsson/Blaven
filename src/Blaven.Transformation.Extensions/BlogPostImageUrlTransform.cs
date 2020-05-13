using System;
using AngleSharp.Html.Parser;

namespace Blaven.Transformation.Extensions
{
    public class BlogPostImageUrlTransform
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
            if (!string.IsNullOrWhiteSpace(post.ImageUrl)
                || string.IsNullOrWhiteSpace(post.Content))
            {
                return;
            }

            var imageUrl = GetImageUrl(post);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return;
            }

            post.ImageUrl = imageUrl;
        }

        private string GetImageUrl(BlogPost post)
        {
            var htmlParser = new HtmlParser();

            var document = htmlParser.ParseDocument(post.Content);

            return document.QuerySelector("img")?.GetAttribute("src");
        }
    }
}

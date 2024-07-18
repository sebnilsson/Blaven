using System;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Blaven.Transformation.Extensions
{
    public class BlogPostHeaderIncreaseTransform(bool addClassName)
                : IBlogPostQueryTransform, IBlogPostStorageTransform
    {
        private readonly bool _addClassName = addClassName;

        public BlogPostHeaderIncreaseTransform()
            : this(addClassName: true)
        {

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
            if (string.IsNullOrWhiteSpace(post.Content))
            {
                return;
            }

            var headerIncreasedContent = GetHeaderIncreasedContent(post);

            post.Content = headerIncreasedContent;
        }

        private string GetHeaderIncreasedContent(BlogPost post)
        {
            var htmlParser = new HtmlParser();

            var document = htmlParser.ParseDocument(post.Content);

            IncreaseHeader(document, 5);
            IncreaseHeader(document, 4);
            IncreaseHeader(document, 3);
            IncreaseHeader(document, 2);
            IncreaseHeader(document, 1);

            return document.Body?.InnerHtml ?? post.Content;
        }

        private void IncreaseHeader(IHtmlDocument document, int headerLevel)
        {
            var sourceHeaderTag = $"h{headerLevel}";
            var targetHeaderTag = $"h{headerLevel + 1}";

            var headerTags = document.QuerySelectorAll(sourceHeaderTag);

            foreach (var tag in headerTags)
            {
                var createdTag = document.CreateElement(targetHeaderTag);
                createdTag.InnerHtml = tag.InnerHtml;

                if (_addClassName)
                {
                    createdTag.ClassList.Add(sourceHeaderTag);
                }

                tag.Replace(createdTag);
            }
        }
    }
}

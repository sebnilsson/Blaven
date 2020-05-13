using System;

namespace Blaven.Transformation
{
    public static class BlogPostListTransformExtensions
    {
        public static IPagedReadOnlyList<BlogPostHeader> TryTransformPostHeaders(
            this IPagedReadOnlyList<BlogPostHeader> postHeaders,
            IBlogPostQueryTransformService transformService)
        {
            if (postHeaders is null)
                throw new ArgumentNullException(nameof(postHeaders));
            if (transformService is null)
                throw new ArgumentNullException(nameof(transformService));

            foreach (var postHeader in postHeaders)
            {
                transformService.TransformPostHeader(postHeader);
            }

            return postHeaders;
        }
    }
}

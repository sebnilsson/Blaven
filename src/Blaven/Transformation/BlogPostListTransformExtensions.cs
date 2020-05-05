using System;
using System.Collections.Generic;

namespace Blaven.Transformation
{
    public static class BlogPostListTransformExtensions
    {
        public static IReadOnlyList<BlogPost> TryTransformPosts(
            this IReadOnlyList<BlogPost> posts,
            IBlogPostQueryTransformService transformService)
        {
            if (transformService is null)
                throw new ArgumentNullException(nameof(transformService));

            foreach (var post in posts)
            {
                transformService.TransformPost(post);
            }

            return posts;
        }

        public static IReadOnlyList<BlogPostHeader> TryTransformPostHeaders(
            this IReadOnlyList<BlogPostHeader> postHeaders,
            IBlogPostQueryTransformService transformService)
        {
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

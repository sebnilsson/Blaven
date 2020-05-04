using System;

namespace Blaven.Transformation
{
    internal static class BlogPostTransformExtensions
    {
        public static BlogPost? TryTransformPost(
            this BlogPost? post,
            IBlogPostQueryTransformService transformService)
        {
            if (transformService is null)
                throw new ArgumentNullException(nameof(transformService));

            if (post == null)
            {
                return null;
            }

            transformService.TransformPost(post);

            return post;
        }
    }
}

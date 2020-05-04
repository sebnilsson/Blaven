using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Transformation
{
    public class BlogPostQueryTransformService
        : IBlogPostQueryTransformService
    {
        private readonly IReadOnlyList<IBlogPostQueryTransform> _transforms;

        public BlogPostQueryTransformService(
            IEnumerable<IBlogPostQueryTransform> transforms)
        {
            if (transforms is null)
                throw new ArgumentNullException(nameof(transforms));

            _transforms = transforms.ToList();
        }

        public void TransformPost(BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            foreach (var transform in _transforms)
            {
                transform.Transform(post);
            }
        }

        public void TransformPostHeader(BlogPostHeader postHeader)
        {
            if (postHeader is null)
                throw new ArgumentNullException(nameof(postHeader));

            foreach (var transform in _transforms)
            {
                transform.Transform(postHeader);
            }
        }
    }
}

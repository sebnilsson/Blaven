using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Transformation
{
    public class BlogPostStorageTransformService
        : IBlogPostStorageTransformService
    {
        private readonly IReadOnlyList<IBlogPostStorageTransform> _transforms;

        public BlogPostStorageTransformService(
            IEnumerable<IBlogPostStorageTransform> transforms)
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
    }
}

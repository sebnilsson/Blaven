using System;
using System.Collections.Generic;

namespace Blaven.Transformers
{
    public class BlogPostTransformerService : IBlogPostTransformerService
    {
        private readonly IEnumerable<IBlogPostTransformer> _transformers;

        public BlogPostTransformerService(
            IEnumerable<IBlogPostTransformer> transformers)
        {
            _transformers = transformers
                ?? throw new ArgumentNullException(nameof(transformers));
        }

        public void Apply(BlogPost blogPost)
        {
            if (blogPost is null)
                throw new ArgumentNullException(nameof(blogPost));

            foreach (var transformer in _transformers)
            {
                transformer.Transform(blogPost);
            }
        }
    }
}

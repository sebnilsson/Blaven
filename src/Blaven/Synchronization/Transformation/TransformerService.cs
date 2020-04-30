using System;
using System.Collections.Generic;

namespace Blaven.Synchronization.Transformation
{
    public class TransformerService : ITransformerService
    {
        private readonly IEnumerable<IBlogPostTransformer> _transformers;

        public TransformerService(
            IEnumerable<IBlogPostTransformer> transformers)
        {
            _transformers = transformers
                ?? throw new ArgumentNullException(nameof(transformers));
        }

        public void TransformPost(BlogPost post)
        {
            if (post is null)
                throw new ArgumentNullException(nameof(post));

            foreach (var transformer in _transformers)
            {
                transformer.Transform(post);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven.Synchronization.Transformation
{
    public class TransformerService : ITransformerService
    {
        private readonly IReadOnlyList<IBlogPostTransformer> _transformers;

        public TransformerService(
            IEnumerable<IBlogPostTransformer> transformers)
        {
            if (transformers is null)
                throw new ArgumentNullException(nameof(transformers));

            _transformers = transformers.ToList();
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

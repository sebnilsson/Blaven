using System;
using System.Collections.Generic;

namespace Blaven.Transformers
{
    public class BlogTransformersProvider
    {
        public BlogTransformersProvider(IEnumerable<IBlogPostTransformer> transformers)
        {
            if (transformers == null)
            {
                throw new ArgumentNullException(nameof(transformers));
            }

            this.Transformers = new List<IBlogPostTransformer>(transformers);
        }

        public List<IBlogPostTransformer> Transformers { get; }

        public BlogPost ApplyTransformers(BlogPost blogPost)
        {
            if (blogPost == null)
            {
                throw new ArgumentNullException(nameof(blogPost));
            }

            this.Transformers.ForEach(transformer => transformer.Transform(blogPost));

            return blogPost;
        }

        internal static IEnumerable<IBlogPostTransformer> GetDefaultTransformers()
        {
            yield return new PhraseTagsTransformer();
        }
    }
}